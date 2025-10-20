#!/bin/bash

. utils.sh

ORG=$1
USERNAME=$USER
BACKUP_FILE=$2
REMOTE=${3:-0}
DATETIME=${4:-$(date +%Y%m%d-%H%M%S)}
WORKING_DIR="$(dirname "$(readlink -f "$0")" | sed 's/\/fabric-network//')"
TMP_DIR=/tmp/hf_restore
REQUIRED_DIRS=("organizations" "channel-artifacts" "chain-data" "config" "env" "systemd" "packagedChaincode")

restore() {
    ORG=$1
    BACKUP_FILE=$2
    
    if [ ! -f "$BACKUP_FILE" ]; then
        errorln "Error: Backup file not found: $BACKUP_FILE"
        exit 1
    fi

    infoln "Extract backup data..."
    rm -rf "$TMP_DIR" && mkdir -p "$TMP_DIR"
    tar -xzf $BACKUP_FILE -C $TMP_DIR
    tar -xzf $WORKING_DIR/restores/$DATETIME/etcdraft.tar.gz -C $TMP_DIR

    MISSING=0
    for DIR in "${REQUIRED_DIRS[@]}"; do
        if [ ! -d "$TMP_DIR/$DIR" ]; then
            errorln "Error: Required directory missing in backup: $TMP_DIR/$DIR"
            MISSING=1
        fi
    done

    if [ $MISSING -eq 1 ]; then
        errorln "Restore aborted due to missing directories in backup."
        rm -rf "$TMP_DIR"
        exit 1
    fi
    
    infoln "Cleanup crypto material..."
    for DIR in "${REQUIRED_DIRS[@]}"; do
        if [ -d "./$DIR" ]; then
            runAsRoot rm -rf "./$DIR"
        fi
    done

    runAsRoot cp -r $TMP_DIR/systemd .
    infoln "Setup systemd services"
    ./00.setup.sh $ORG

    infoln "Stopping peer..."
    servicesManagement stop $ORG

    infoln "Restore crypto material..."
    for DIR in "${REQUIRED_DIRS[@]}"; do
        if [ "$DIR" != "systemd" ]; then
            runAsRoot cp -r $TMP_DIR/$DIR .
        fi
    done
    runAsRoot cp -r $TMP_DIR/wal $WORKING_DIR/fabric-network/chain-data/ordererOrganizations/org$ORG.atgdigitals.com/etcdraft/
    runAsRoot cp -r $TMP_DIR/snapshot $WORKING_DIR/fabric-network/chain-data/ordererOrganizations/org$ORG.atgdigitals.com/etcdraft/

    infoln "Starting peer..."
    servicesManagement restart $ORG

    runAsRoot rm -rf "$TMP_DIR"
    sleep 5
    # healthCheck $ORG
    successln "Restore completed"
}

if [[ "$REMOTE" == "0" ]]; then
    WORKING_ORG=$(basename $(ls -d $WORKING_DIR/fabric-network/chain-data/ordererOrganizations/org*.atgdigitals.com | head -n1))
    WORKING_ORG=${WORKING_ORG//[^0-9]/}

    runAsRoot cp -r $WORKING_DIR/fabric-network/chain-data/ordererOrganizations/org$WORKING_ORG.atgdigitals.com/etcdraft $TMP_DIR
    runAsRoot tar -czf "$TMP_DIR/etcdraft.tar.gz" -C $TMP_DIR .

    ssh $USERNAME@peer0.org$ORG.atgdigitals.com "mkdir -p {$WORKING_DIR/fabric-network,$WORKING_DIR/restores/$DATETIME}"
    scp ./{restore.sh,00.setup.sh,utils.sh} $USERNAME@peer0.org$ORG.atgdigitals.com:$WORKING_DIR/fabric-network
    scp -r ./bin ./builders $USERNAME@peer0.org$ORG.atgdigitals.com:$WORKING_DIR/fabric-network
    scp $BACKUP_FILE $USERNAME@peer0.org$ORG.atgdigitals.com:$WORKING_DIR/restores/$DATETIME
    scp $TMP_DIR/etcdraft.tar.gz $USERNAME@peer0.org$ORG.atgdigitals.com:$WORKING_DIR/restores/$DATETIME
    ssh $USERNAME@peer0.org$ORG.atgdigitals.com "cd $WORKING_DIR/fabric-network && ./restore.sh $ORG $BACKUP_FILE 1 $DATETIME"
    runAsRoot rm -rf $TMP_DIR 
else
    restore $ORG $BACKUP_FILE
fi
