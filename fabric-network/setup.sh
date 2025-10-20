#!/bin/bash

. utils.sh

USERNAME=$USER
WORKING_DIR="$(dirname "$(readlink -f "$0")" | sed 's/\/hyperledger\/fabric-network//')"

# Clear all data and reinitialize folder structure
./00.clear_all.sh $USERNAME

# Setup environment
./00.setup.sh 1
ssh $USERNAME@peer0.org2.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo bash ./00.setup.sh 2"
ssh $USERNAME@peer0.org3.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo bash ./00.setup.sh 3"

# Initialize TLS CA servers
./02.start_tls_ca_server.sh stop
./04.start_peer_and_ordering.sh stop org1
ssh $USERNAME@peer0.org2.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo ./04.start_peer_and_ordering.sh stop org2"
ssh $USERNAME@peer0.org3.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo ./04.start_peer_and_ordering.sh stop org3"
./04.start_peer_and_ordering.sh stop orderer1
ssh $USERNAME@peer0.org2.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo ./04.start_peer_and_ordering.sh stop orderer2"
ssh $USERNAME@peer0.org3.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo ./04.start_peer_and_ordering.sh stop orderer3"
./01.init_tls_ca_server.sh all
./02.start_tls_ca_server.sh start
./03.enroll_servers.sh
./02.start_tls_ca_server.sh stop

# Copy certificate files to peer0.org2 and peer0.org3
scp -r ./organizations $USERNAME@peer0.org2.atgdigitals.com:$WORKING_DIR/hyperledger/fabric-network
scp -r ./organizations $USERNAME@peer0.org3.atgdigitals.com:$WORKING_DIR/hyperledger/fabric-network

# Start all servers
./04.start_peer_and_ordering.sh start org1 
ssh $USERNAME@peer0.org2.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo bash ./04.start_peer_and_ordering.sh start org2"
ssh $USERNAME@peer0.org3.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo bash ./04.start_peer_and_ordering.sh start org3"
./04.start_peer_and_ordering.sh start orderer1 
ssh $USERNAME@peer0.org2.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo bash ./04.start_peer_and_ordering.sh start orderer2"
ssh $USERNAME@peer0.org3.atgdigitals.com "cd $WORKING_DIR/hyperledger/fabric-network && sudo bash ./04.start_peer_and_ordering.sh start orderer3"

scp $USERNAME@peer0.org2.atgdigitals.com:$WORKING_DIR/hyperledger/fabric-network/config/org2/core.yaml $WORKING_DIR/hyperledger/fabric-network/config/org2/core.yaml
scp $USERNAME@peer0.org3.atgdigitals.com:$WORKING_DIR/hyperledger/fabric-network/config/org3/core.yaml $WORKING_DIR/hyperledger/fabric-network/config/org3/core.yaml

# Create channel and deploy chaincode
./05.create_channel.sh
./06.deploy_chaincode.sh ../asset-transfer-basic/chaincode-go peer0.org1.atgdigitals.com peer0.org2.atgdigitals.com peer0.org3.atgdigitals.com $USERNAME $WORKING_DIR
scp -r $WORKING_DIR/hyperledger/fabric-network/{channel-artifacts,packagedChaincode} $USERNAME@peer0.org2.atgdigitals.com:$WORKING_DIR/hyperledger/fabric-network
scp -r $WORKING_DIR/hyperledger/fabric-network/{channel-artifacts,packagedChaincode} $USERNAME@peer0.org3.atgdigitals.com:$WORKING_DIR/hyperledger/fabric-network

# Get peer connections configuration
./07.collect_connection_configurations.sh remote $USERNAME
