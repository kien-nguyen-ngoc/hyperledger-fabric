#!/bin/bash

. utils.sh

ORG=$1
USERNAME=$2
DATETIME=${3:-$(date +%Y%m%d-%H%M%S)}
REMOTE_BACKUP=${4:-0}
BACKUP_ROOT=${5:-${PWD}/../backups}
TMP_DIR="/tmp/$DATETIME"
FABRIC_DATA=${5:-${PWD}}
REQUIRED_DIRS=("organizations" "channel-artifacts" "chain-data" "config" "env" "systemd" "packagedChaincode")


backup() {
    # Check all required directories first
    MISSING=0
    for DIR in "${REQUIRED_DIRS[@]}"; do
        if [ ! -d "$FABRIC_DATA/$DIR" ]; then
            errorln "Error: Required directory not found: $FABRIC_DATA/$DIR"
            MISSING=1
        fi
    done

    if [ $MISSING -eq 1 ]; then
        errorln "Backup aborted due to missing directories."
        exit 1
    fi

    infoln "Starting Fabric backup at $TMP_DIR ..."
    mkdir -p "$TMP_DIR"

    for DIR in "${REQUIRED_DIRS[@]}"; do
        echo "* - Backing up $DIR..."
        runAsRoot cp -r $FABRIC_DATA/$DIR $TMP_DIR/
    done

    # Archive everything
    infoln "* - Archiving backup..."
    DATE=${DATETIME%%-*}
    ARCHIVE_NAME=$BACKUP_ROOT/$DATE/$DATETIME-svr$ORG.tar.gz
    echo $BACKUP_ROOT/$DATE
    mkdir -p $BACKUP_ROOT/$DATE
    runAsRoot tar -czf "$ARCHIVE_NAME" -C $TMP_DIR .
    runAsRoot rm -rf "$TMP_DIR"

    successln "Backup completed for server: org$ORG.atgdigitals.com"
}

remote_backup() {
    ORG=$1
    scp ./00.backup.sh $USERNAME@peer0.org$ORG.atgdigitals.com:$PWD
    ssh $USERNAME@peer0.org$ORG.atgdigitals.com "cd $PWD && ./00.backup.sh $ORG $USERNAME $DATETIME 1"
    scp $USERNAME@peer0.org$ORG.atgdigitals.com:$BACKUP_ROOT/$DATE/$DATETIME-svr$ORG.tar.gz $BACKUP_ROOT/$DATE
    ssh $USERNAME@peer0.org$ORG.atgdigitals.com "cd $BACKUP_ROOT/ && rm -rf $DATE"
}

backup $ORG

ORGS=("1" "2" "3")
for O in "${ORGS[@]}"; do
    if [[ "$REMOTE_BACKUP" == "0" && "$O" != "$ORG" ]]; then
        infoln "Remote backup for server: org$O.atgdigitals.com"
        remote_backup $O
    fi
done

if [[ "$REMOTE_BACKUP" == "0" ]]; then
    ARCHIVE_NAME=$BACKUP_ROOT/$DATE.tar.gz
    tar -czf "$ARCHIVE_NAME" -C $BACKUP_ROOT/$DATE .
    runAsRoot rm -rf $BACKUP_ROOT/$DATE
fi