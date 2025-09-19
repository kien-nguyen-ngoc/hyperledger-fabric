
./init-all.sh <username> <root-dir>
./init-all.sh sysadmin /home/hyperledger

Deploy hyperledger trên 2 node, gồm 1 orderer + 2 peer
    Server 1: 
        Roles: orderer, peer 1
        Services: 
            fabric-ordering.service
            fabric-peer-org1.service
            fabric-gateway.service
            fabric-chaincode.service
    Server 2: 
        Roles: peer 2
        Service: 
            fabric-peer-org2.service

Storage:
    TLS certificates:
        organizations
    Blockchain data:
        channel-artifacts
        chain-data
    Node configurations:
        config
