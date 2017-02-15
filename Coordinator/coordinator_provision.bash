#!/bin/bash

SERVICE_USER="ubuntu"
SERVICE_PORT=8888
KEY="kolejki"

PG_IP=test.cdylfzlzer8p.eu-west-1.rds.amazonaws.com
PG_USER=test
PG_DB=test
PG_PASS=testtest
PG_PORT=5432
REDIS_IP=queueredis.y6w0go.0001.euw1.cache.amazonaws.com
REDIS_PORT=6379

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

COORDINATOR_DIR="$PROJECT_DIR/Coordinator"

cd $COORDINATOR_DIR

COORDINATOR_SLN="coordinator.sln"

nuget restore $COORDINATOR_SLN
xbuild /p:Configuration=Release $COORDINATOR_SLN

ENV_FILE="$COORDINATOR_DIR/server/bin/Release/.env"

echo "SERVICE_PORT=$SERVICE_PORT" > $ENV_FILE
echo "KEY=$KEY" >> $ENV_FILE
echo "PG_IP=$PG_IP" >> $ENV_FILE
echo "PG_USER=$PG_USER" >> $ENV_FILE
echo "PG_DB=$PG_DB" >> $ENV_FILE
echo "PG_PASS=$PG_PASS" >> $ENV_FILE
echo "PG_PORT=$PG_PORT" >> $ENV_FILE
echo "REDIS_IP=$REDIS_IP" >> $ENV_FILE
echo "REDIS_PORT=$REDIS_PORT" >> $ENV_FILE

chown -R $SERVICE_USER:$SERVICE_USER $PROJECT_DIR

systemctl enable coordinator
systemctl start coordinator
