#!/bin/bash

. utils.sh

MODE=$1
USERNAME=$2

chmod +x $PWD/bin/chaincode/assetTransfer

if [ "$MODE" == "start" ]; then
  runAsRoot systemctl restart fabric-gateway.service
  exit 0
fi


rm -rf $PWD/../gateway-resources
mkdir -p $PWD/../gateway-resources/{peer1,peer2,peer3}

cp $PWD/organizations/peerOrganizations/org1.atgdigitals.com/users/User1@org1.atgdigitals.com/msp/signcerts/cert.pem $PWD/../gateway-resources/peer1/cert.pem
cp -r $PWD/organizations/peerOrganizations/org1.atgdigitals.com/users/User1@org1.atgdigitals.com/msp/keystore $PWD/../gateway-resources/peer1/keystore
cp $PWD/organizations/peerOrganizations/org1.atgdigitals.com/peers/peer0.org1.atgdigitals.com/tls/ca.crt $PWD/../gateway-resources/peer1/ca.crt

scp $USERNAME@peer0.org2.atgdigitals.com:$PWD/organizations/peerOrganizations/org2.atgdigitals.com/users/User1@org2.atgdigitals.com/msp/signcerts/cert.pem $PWD/../gateway-resources/peer2/cert.pem
scp -r $USERNAME@peer0.org2.atgdigitals.com:$PWD/organizations/peerOrganizations/org2.atgdigitals.com/users/User1@org2.atgdigitals.com/msp/keystore $PWD/../gateway-resources/peer2/keystore
scp $USERNAME@peer0.org2.atgdigitals.com:$PWD/organizations/peerOrganizations/org2.atgdigitals.com/peers/peer0.org2.atgdigitals.com/tls/ca.crt $PWD/../gateway-resources/peer2/ca.crt

scp $USERNAME@peer0.org3.atgdigitals.com:$PWD/organizations/peerOrganizations/org3.atgdigitals.com/users/User1@org3.atgdigitals.com/msp/signcerts/cert.pem $PWD/../gateway-resources/peer3/cert.pem
scp -r $USERNAME@peer0.org3.atgdigitals.com:$PWD/organizations/peerOrganizations/org3.atgdigitals.com/users/User1@org3.atgdigitals.com/msp/keystore $PWD/../gateway-resources/peer3/keystore
scp $USERNAME@peer0.org3.atgdigitals.com:$PWD/organizations/peerOrganizations/org3.atgdigitals.com/peers/peer0.org3.atgdigitals.com/tls/ca.crt $PWD/../gateway-resources/peer3/ca.crt
