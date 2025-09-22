#!/bin/bash

. ./utils.sh

ORG=$1

set -x

sudo cp $PWD/systemd/*/*.service  /etc/systemd/system/

sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-ca-ordererOrg1.service
sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-ca-ordererOrg2.service
sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-ca-ordererOrg3.service
sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-ca-org1.service
sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-ca-org2.service
sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-ca-org3.service
sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-ordering$ORG.service
sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-peer-org$ORG.service
sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-chaincode.service
sudo sed -i "s|\$PWD|$PWD|g" /etc/systemd/system/fabric-gateway.service
sudo sed -i "s|fabric-network/application-gateway-go|asset-transfer-basic/application-gateway-go|" /etc/systemd/system/fabric-gateway.service

sudo bash -c "systemctl daemon-reload"
sudo bash -c "systemctl enable fabric-ordering$ORG.service"
sudo bash -c "systemctl enable fabric-peer-org$ORG.service"
sudo bash -c "systemctl enable fabric-chaincode.service"

chmod +x -R $PWD/bin/*
chmod +x -R $PWD/builders/*
chmod +x -R $PWD/*.sh

{ set +x; } 2>/dev/null
