#!/bin/bash

WORKER_ID=
SERVICE_INTERFACE="eth0"
SERVICE_PORT=
SERVICE_USER=""
COORDINATOR_ADDRESS=""
APP_KEY=""

PROJECT_REPO_NAME="Messages-queue"
PROJECT_REPO="https://github.com/Artii15/$PROJECT_REPO_NAME.git"
PROJECT_DIR_PWD="/home/$SERVICE_USER"
PROJECT_DIR="$PROJECT_DIR_PWD/$PROJECT_REPO_NAME"

if [ -d $PROJECT_DIR ]; then
    cd $PROJECT_DIR
    git pull origin master
else
    cd $PROJECT_DIR_PWD
    git clone $PROJECT_REPO
fi

WORKER_DIR="$PROJECT_DIR/Worker"

cd $WORKER_DIR

WORKER_SLN="worker.sln"

nuget restore $WORKER_SLN
xbuild /p:Configuration=Release $WORKER_SLN

ENV_FILE="$WORKER_DIR/server/bin/Release/.env"

echo "WORKER_ID=$WORKER_ID" > $ENV_FILE
echo "COORDINATOR_ADDRESS=$COORDINATOR_ADDRESS" >> $ENV_FILE
echo "SERVICE_INTERFACE=$SERVICE_INTERFACE" >> $ENV_FILE
echo "SERVICE_PORT=$SERVICE_PORT" >> $ENV_FILE
echo "APP_KEY=$APP_KEY" >> $ENV_FILE

chown -R $SERVICE_USER:$SERVICE_USER $PROJECT_DIR

systemctl enable worker
systemctl start worker
