#!/bin/bash

chmod +x $PWD/bin_chaincode/assetTransfer

sudo bash -c "systemctl restart fabric-gateway.service"
