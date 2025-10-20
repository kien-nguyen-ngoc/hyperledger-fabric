## Requirements:
- ssh across all servers
- sudo without password
- command jq and tar

## Installation
```shell
./setup.sh
```
e.g., ./setup.sh

### Verify successful installation
Run commands:
```shell
./999.status.sh 1
./999.status.sh 2
./999.status.sh 3
```
Expected result:
```
[chaincodeCmd] chaincodeInvokeOrQuery -> Chaincode invoke successful. result: status:200
```

## Backup
At working peer run command:
```shell
./backup.sh
```
e.g., ./backup.sh

## Restore
At working peer "X" run command:
```shell
./restore.sh <X> <package>
```
e.g., ./restore.sh 3 restores/20250922-093426-svr1.tar.gz

## Component Services 
- fabric-chaincode.service
- fabric-peer-org1.service
- fabric-ordering1.service
