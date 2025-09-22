#!/bin/bash

. utils.sh

chmod +x $PWD/bin/chaincode/assetTransfer

runAsRoot systemctl restart fabric-gateway.service
