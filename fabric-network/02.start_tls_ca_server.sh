#!/bin/bash

. utils.sh

set -x

MODE=${1:-status}

runAsRoot systemctl $MODE fabric-ca-ordererOrg1.service
runAsRoot systemctl $MODE fabric-ca-org1.service

runAsRoot systemctl $MODE fabric-ca-ordererOrg2.service
runAsRoot systemctl $MODE fabric-ca-org2.service

runAsRoot systemctl $MODE fabric-ca-ordererOrg3.service
runAsRoot systemctl $MODE fabric-ca-org3.service

sleep 3

{ set +x; } 2>/dev/null
