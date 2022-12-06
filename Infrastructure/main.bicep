param location string = resourceGroup().location
param envName string
param appName string
param containerImage string
param containerPort int = 80
@secure()
param acrPassword string
param acrUsername string
param acrName string
var stackname = '${appName}-${envName}'

@secure()
param eventhubconnectionstring string

@secure()
param storageAccountKey string

module law 'log-analytics.bicep' = {
	name: 'log-analytics-workspace'
	params: {
      location: location
      name: '${stackname}-law'
	}
}

module containerAppEnvironment 'aca-environment.bicep' = {
  name: 'container-app-environment'
  params: {
    name: stackname
    location: location
    
    lawClientId:law.outputs.clientId
    lawClientSecret: law.outputs.clientSecret
  }
}

resource daprPubSub 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: '${envName}/pubsub'
  properties: {
    componentType: 'bindings.azure.eventhubs'
    version: 'v1'
    metadata: [
      {
        name: 'connectionString'
        secretRef: 'eventhubconnectionstring'
      }
      {
        name: 'consumerGroup'
        value: 'cg001'
      }
      {
        name: 'storageAccountName'
        value: 'azeventhubcp'
      }
      {
        name: 'storageAccountKey'
        secretRef: 'storageAccountKey'
      }
      {
        name: 'storageContainerName '
        secretRef: 'checkpoint'
      }
    ]
    scopes: [ 'trafficcontrolservice' ]
  }
}

module containerApp 'aca.bicep' = {
  name: 'container-app'
  params: {
    name: stackname
    location: location
    containerAppEnvironmentId: containerAppEnvironment.outputs.id
    containerImage: containerImage
    envVars: []
    useExternalIngress: true
    containerPort: containerPort
    acrPassword: acrPassword
    acrUsername: acrUsername
    acrName: acrName
    eventhubconnectionstring: eventhubconnectionstring
    storageAccountKey: storageAccountKey
  }
}

output fqdn string = containerApp.outputs.fqdn
