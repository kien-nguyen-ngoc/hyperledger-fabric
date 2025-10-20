#!/bin/bash

. ./utils.sh

set -x

USERNAME=$1
WORKING_DIR="$(dirname "$(readlink -f "$0")" | sed 's/\/hyperledger\/fabric-network//')"

runAsRoot rm -rf organizations
runAsRoot rm -rf channel-artifacts
runAsRoot rm -rf chain-data
runAsRoot rm -rf packagedChaincode
runAsRoot rm -rf config/ordererOrg1/orderer.yaml
runAsRoot rm -rf config/ordererOrg2/orderer.yaml
runAsRoot rm -rf config/ordererOrg3/orderer.yaml
runAsRoot rm -rf config/org1/core.yaml
runAsRoot rm -rf config/org2/core.yaml
runAsRoot rm -rf config/org3/core.yaml

runAsRoot systemctl stop fabric-*
ssh $USERNAME@peer0.org2.atgdigitals.com "bash -c 'sudo systemctl stop fabric-*'"
ssh $USERNAME@peer0.org2.atgdigitals.com "sudo rm -rf $WORKING_DIR/hyperledger && mkdir -p $WORKING_DIR/hyperledger/fabric-network"
scp -r $WORKING_DIR/hyperledger/fabric-network $USERNAME@peer0.org2.atgdigitals.com:$WORKING_DIR/hyperledger
ssh $USERNAME@peer0.org3.atgdigitals.com "bash -c 'sudo systemctl stop fabric-*'"
ssh $USERNAME@peer0.org3.atgdigitals.com "sudo rm -rf $WORKING_DIR/hyperledger && mkdir -p $WORKING_DIR/hyperledger/fabric-network"
scp -r $WORKING_DIR/hyperledger/fabric-network $USERNAME@peer0.org3.atgdigitals.com:$WORKING_DIR/hyperledger

{ set +x; } 2>/dev/null
