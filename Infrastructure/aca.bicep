param location string
param name string
param containerAppEnvironmentId string

// Container Image ref
param containerImage string

// Networking
param useExternalIngress bool = false
param containerPort int

param envVars array = []

param acrName string
param acrUsername string
@secure()
param acrPassword string

@secure()
param eventhubconnectionstring string

@secure()
param storageAccountKey string

var containerImageParts = split(containerImage, ':')

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: name
  location: location
  properties: {
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      secrets: [
        {
          name: 'acrpassword'
          value: acrPassword
        }
        {
          name: 'eventhubconnectionstring'
          value: eventhubconnectionstring
        }
        {
          name: 'storageaccountkey'
          value: storageAccountKey
        }
      ]
      registries: [
        {
          server: '${acrName}.azurecr.io'
          username: acrUsername
          passwordSecretRef: 'acrpassword'
        }
      ]
      ingress: {
        external: useExternalIngress
        targetPort: containerPort
      }
    }
    template: {
      containers: [
        {
          image: containerImage
          name: name
          env: [
            {
              name: 'APPLICATION_VERSION'
              value: containerImageParts[1]
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 10
        rules: [ {
            name: 'cpu-scaling-rule'
            custom: {
              type: 'cpu'
              metadata: {
                type: 'Utilization'
                value: '75'
              }
            }
          } ]
      }
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
