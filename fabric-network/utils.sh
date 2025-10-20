#!/bin/bash

C_RESET='\033[0m'
C_RED='\033[0;31m'
C_GREEN='\033[0;32m'
C_BLUE='\033[0;34m'
C_YELLOW='\033[1;33m'

function println() {
  echo -e "$1"
}

function errorln() {
  println "${C_RED}${1}${C_RESET}"
}

function successln() {
  println "${C_GREEN}${1}${C_RESET}"
}

function infoln() {
  println "${C_BLUE}${1}${C_RESET}"
}

function fatalln() {
  errorln "$1"
  exit 1
}

function verifyResult() {
  if [ $1 -ne 0 ]; then
    errorln "$2"
    exit 1
  fi
}

function runAsNonRoot() {
  CMD=$1
  shift  
  
  infoln "Running as non-root user ($SUDO_USER) command: $CMD $*"
  cd $(pwd) && $CMD $@
}

function runAsRoot() {
  CMD=$1
  shift  
  
  infoln "Running as root user command: $CMD $*"
  sudo bash -c "cd $(pwd) && $CMD $*"
}

function parsePeerConnectionParameters() {
  PEER_CONN_PARMS=()
  PEERS=""
  while [ "$#" -gt 0 ]; do
    ORG=$1
    HOST=$2
    export FABRIC_CFG_PATH=$PWD/config/org$ORG
    export CORE_PEER_TLS_ROOTCERT_FILE=${PWD}/organizations/peerOrganizations/org${ORG}.atgdigitals.com/tlsca/tlsca.org${ORG}.atgdigitals.com-cert.pem
    export CORE_PEER_MSPCONFIGPATH=${PWD}/organizations/peerOrganizations/org${ORG}.atgdigitals.com/users/Admin@org${ORG}.atgdigitals.com/msp
    export CORE_PEER_ADDRESS=$HOST:7051

    PEER="peer0.org$1"
    ## Set peer addresses
    if [ -z "$PEERS" ]
    then
	PEERS="$PEER"
    else
	PEERS="$PEERS $PEER"
    fi
    PEER_CONN_PARMS=("${PEER_CONN_PARMS[@]}" --peerAddresses $CORE_PEER_ADDRESS)
    ## Set path to TLS certificate
    CA=${PWD}/organizations/peerOrganizations/org$1.atgdigitals.com/tlsca/tlsca.org$1.atgdigitals.com-cert.pem
    TLSINFO=(--tlsRootCertFiles "$CA")
    PEER_CONN_PARMS=("${PEER_CONN_PARMS[@]}" "${TLSINFO[@]}")
    # shift by two to get to the next organization
    shift
    shift
  done
}

function servicesManagement() {
  MODE=$1
  ORG=$2
  runAsRoot systemctl $MODE fabric-chaincode.service
  runAsRoot systemctl $MODE fabric-peer-org$ORG.service
  runAsRoot systemctl $MODE fabric-ordering$ORG.service
  sleep 3
}

function healthCheck() {
  ORG=${1:-1}
  export FABRIC_CFG_PATH=$PWD/config/org$ORG
  export CA_FILE="$PWD/organizations/ordererOrganizations/org$ORG.atgdigitals.com/tlsca/tlsca.org$ORG.atgdigitals.com-cert.pem"
  export CORE_TLS_CLIENT_CERT_FILE="$PWD/organizations/peerOrganizations/org$ORG.atgdigitals.com/peers/peer0.org$ORG.atgdigitals.com/tls/server.crt"
  export CORE_TLS_CLIENT_KEY_PATH="$PWD/organizations/peerOrganizations/org$ORG.atgdigitals.com/peers/peer0.org$ORG.atgdigitals.com/tls/server.key"
  export CORE_PEER_TLS_ROOTCERT_FILE="$PWD/organizations/peerOrganizations/org$ORG.atgdigitals.com/peers/peer0.org$ORG.atgdigitals.com/tls/ca.crt"
  export CORE_PEER_MSPCONFIGPATH="$PWD/organizations/peerOrganizations/org$ORG.atgdigitals.com/users/Admin@org$ORG.atgdigitals.com/msp"
  export CORE_PEER_TLS_ENABLED="true"
  export CORE_PEER_LOCALMSPID="Org${ORG}MSP"
  export CORE_PEER_ADDRESS="peer0.org$ORG.atgdigitals.com:7051"

  ./bin/fabric/peer lifecycle chaincode querycommitted --channelID default-channel
  ./bin/fabric/peer chaincode invoke \
                  -C default-channel \
                  -n chaincode-go-1-0 \
                  -o orderer${ORG}.atgdigitals.com:7050 \
                  --peerAddresses $CORE_PEER_ADDRESS \
                  --tlsRootCertFiles $CORE_PEER_TLS_ROOTCERT_FILE \
                  --tls \
                  --cafile $CA_FILE \
                  -c '{"Args":["GetAllAccounts"]}'
}

export CORE_PEER_TLS_ENABLED=true
export FABRIC_LOGGING_SPEC=debug:cauthdsl,policies,msp,grpc,peer.gossip.mcs,gossip,leveldbhelper=info

export -f errorln
export -f successln
export -f infoln
export -f fatalln
export -f verifyResult
export -f runAsNonRoot
export -f runAsRoot
export -f parsePeerConnectionParameters
export -f servicesManagement