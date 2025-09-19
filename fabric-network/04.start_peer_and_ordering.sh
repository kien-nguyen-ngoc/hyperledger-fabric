#!/bin/bash

. utils.sh

set -x

MODE=${1:-status}
ORG=$2

function run_peer() {
    ORG=$1 
    local MODE=$2

    if [ "$MODE" == 'status' ]; then
        systemctl status fabric-peer-$ORG.service
        return 0
    fi

    mkdir -p $PWD/chain-data/peerOrganizations/$ORG.atgdigitals.com/snapshots
    sed "s|fileSystemPath: /var/hyperledger/production|fileSystemPath: $PWD/chain-data/peerOrganizations/$ORG.atgdigitals.com|g" $PWD/config/$ORG/core.yaml.template > $PWD/config/$ORG/core.yaml
    sed -i "s|rootDir: /var/hyperledger/production/snapshots|rootDir: $PWD/chain-data/peerOrganizations/$ORG.atgdigitals.com/snapshots|g" $PWD/config/$ORG/core.yaml
    sed -i "s|path: ../../builders/ccaas|path: $PWD/builders/ccaas|g" $PWD/config/$ORG/core.yaml

    sed -i "s|file: tls/server.key|file: $PWD/organizations/peerOrganizations/$ORG.atgdigitals.com/peers/peer0.$ORG.atgdigitals.com/tls/server.key|g" $PWD/config/$ORG/core.yaml
    sed -i "s|file: tls/server.crt|file: $PWD/organizations/peerOrganizations/$ORG.atgdigitals.com/peers/peer0.$ORG.atgdigitals.com/tls/server.crt|g" $PWD/config/$ORG/core.yaml
    sed -i "s|file: tls/ca.crt|file: $PWD/organizations/peerOrganizations/$ORG.atgdigitals.com/peers/peer0.$ORG.atgdigitals.com/tls/ca.crt|g" $PWD/config/$ORG/core.yaml
    sed -i "s|        - tls/ca.crt|        - $PWD/organizations/peerOrganizations/$ORG.atgdigitals.com/peers/peer0.$ORG.atgdigitals.com/tls/ca.crt|g" $PWD/config/$ORG/core.yaml

    systemctl $MODE fabric-peer-$ORG.service
}

function run_ordering() {
    local MODE=$1

    if [ "$MODE" == 'status' ]; then
        systemctl status fabric-ordering.service
        return 0
    fi
    
    mkdir -p $PWD/chain-data/ordererOrganizations/atgdigitals.com/etcdraft/{wal,snapshot}
    sed "s|Location: /var/hyperledger/production/orderer|Location: $PWD/chain-data/ordererOrganizations/atgdigitals.com|g" $PWD/config/ordererOrg/orderer.yaml.template > $PWD/config/ordererOrg/orderer.yaml
    sed -i "s|WALDir: /var/hyperledger/production/orderer/etcdraft/wal|WALDir: $PWD/chain-data/ordererOrganizations/atgdigitals.com/etcdraft/wal|g" $PWD/config/ordererOrg/orderer.yaml
    sed -i "s|SnapDir: /var/hyperledger/production/orderer/etcdraft/snapshot|SnapDir: $PWD/chain-data/ordererOrganizations/atgdigitals.com/etcdraft/snapshot|g" $PWD/config/ordererOrg/orderer.yaml

    sed -i "s|PrivateKey: |PrivateKey: $PWD/organizations/ordererOrganizations/atgdigitals.com/orderers/orderer.atgdigitals.com/tls/server.key|g" $PWD/config/ordererOrg/orderer.yaml
    sed -i "s|Certificate: |Certificate: $PWD/organizations/ordererOrganizations/atgdigitals.com/orderers/orderer.atgdigitals.com/tls/server.crt|g" $PWD/config/ordererOrg/orderer.yaml
    sed -i "s|    RootCAs:|    RootCAs: $PWD/organizations/ordererOrganizations/atgdigitals.com/orderers/orderer.atgdigitals.com/tls/ca.crt|g" $PWD/config/ordererOrg/orderer.yaml
    sed -i "s|ClientRootCAs: \[\]|ClientRootCAs: [$PWD/organizations/ordererOrganizations/atgdigitals.com/orderers/orderer.atgdigitals.com/tls/ca.crt]|g" $PWD/config/ordererOrg/orderer.yaml
    sed -i "s|ListenAddress: 127.0.0.1:9443|ListenAddress: 0.0.0.0:9443|g" $PWD/config/ordererOrg/orderer.yaml
    sed -i "s|ListenAddress: 127.0.0.1|ListenAddress: 0.0.0.0|g" $PWD/config/ordererOrg/orderer.yaml
        
    systemctl $MODE fabric-ordering.service
}

if [[ $ORG == *"org"* ]]; then 
    run_peer $ORG $MODE
    sleep 3
fi 

if [[ $ORG == *"orderer"* ]]; then 
    run_ordering $MODE
    sleep 3
fi

{ set +x; } 2>/dev/null
