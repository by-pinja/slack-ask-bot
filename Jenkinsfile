library 'jenkins-ptcs-library@0.6.0'

podTemplate(label: pod.label,
  containers: pod.templates + [
    containerTemplate(name: 'dotnet', image: 'microsoft/dotnet:3.0.100-preview6-disco', ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
  ]
) {


    node(pod.label) {
        stage('Checkout') {
            checkout scm
        }
        container('dotnet') {
            stage('Build') {
                sh """
                    dotnet build
                """
            }
        }
    }
  }