SERVICE_NAME=$1
SERVICE_PROJECT=$2

docker build -t $SERVICE_NAME src/$SERVICE_PROJECT
cp src/$SERVICE_PROJECT/appsettings.json appsettings/$SERVICE_NAME.json