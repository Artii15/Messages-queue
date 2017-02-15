#!/bin/bash

WORKER_RUNTIME_DIR="/home/ubuntu/Messages-queue/Worker/server/bin/Release"

cd $WORKER_RUNTIME_DIR

while read ENV_VARIABLE
do
    eval $ENV_VARIABLE
done < .env

IP_ADDRESS=$(ip address show dev $SERVICE_INTERFACE | grep inet | grep -v inet6 | sed -e 's/^[ \t]*//' | cut -d " " -f2 | cut -d "/" -f1)

SERVICE_ADDRESS="http://$IP_ADDRESS:$SERVICE_PORT"

export WORKER_ID
export COORDINATOR_ADDRESS
./server.exe $SERVICE_ADDRESS
