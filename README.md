## Requirements:
ssh
sudo
jq
tar

## Installation
```shell
./init-all.sh <username> <root-dir>
```
e.g., ./init-all.sh sysadmin /home/hyperledger

## Backup
At peer X
```shell
./00.backup.sh <X> <username>
```
e.g., ./00.backup.sh 1 sysadmin 

## Restore
At working peer
```shell
./00.restore.sh <org> <username> <package>
```
e.g., ./00.restore.sh 3 sysadmin restores/20250922-093426-svr1.tar.gz


