#!/bin/bash

. utils.sh

ORG=$1
USERNAME=$2
BACKUP_FILE=$3
REMOTE=${4:-0}

TMP_DIR=/tmp/hyperledger_fabric
REQUIRED_DIRS=("organizations" "channel-artifacts" "chain-data" "config" "env" "systemd" "packagedChaincode")

restore() {
    ORG=$1
    BACKUP_FILE=$2
    
    if [ ! -f "$BACKUP_FILE" ]; then
        errorln "Error: Backup file not found: $BACKUP_FILE"
        exit 1
    fi

    infoln "Extract backup data..."
    mkdir -p "$TMP_DIR"
    tar -xzf $BACKUP_FILE -C $TMP_DIR

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
            rm -rf "./$DIR"
        fi
    done

    infoln "Restore crypto material..."
    for DIR in "${REQUIRED_DIRS[@]}"; do
        cp -r $TMP_DIR/$DIR .
    done

    infoln "Starting peer..."
    runAsRoot systemctl restart fabric-chaincode.service
    runAsRoot systemctl restart fabric-peer-org$ORG.service
    runAsRoot systemctl restart fabric-ordering$ORG.service
    runAsRoot systemctl restart fabric-gateway.service

    sleep 5
    checkHealth $ORG
    successln "Restore completed"
}

if [[ "$REMOTE" == "0" ]]; then
    ssh $USERNAME@peer0.org$ORG.atgdigitals.com "mkdir -p {$PWD,$PWD/../restores}"
    scp ./{00.restore.sh,00.setup.sh,utils.sh} $USERNAME@peer0.org$ORG.atgdigitals.com:$PWD
    scp -r ./bin ./builders $USERNAME@peer0.org$ORG.atgdigitals.com:$PWD
    scp $BACKUP_FILE $USERNAME@peer0.org$ORG.atgdigitals.com:$PWD/../restores
    ssh $USERNAME@peer0.org$ORG.atgdigitals.com "cd $PWD && ./00.setup.sh $ORG"
    ssh $USERNAME@peer0.org$ORG.atgdigitals.com "cd $PWD && ./00.restore.sh $ORG $USERNAME $BACKUP_FILE 1"
else
    restore $ORG $BACKUP_FILE
fi
