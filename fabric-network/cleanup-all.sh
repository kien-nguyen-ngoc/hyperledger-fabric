#!/bin/bash

USERNAME=$1
WORKING_DIR=$2

rm -rf $WORKING_DIR/hyperledger
cp -r /mnt/hyperledger $WORKING_DIR/hyperledger

ssh $USERNAME@peer0.org2.atgdigitals.com "bash -c rm -rf $WORKING_DIR/hyperledger"
