library 'jenkins-ptcs-library@3.0.0'

def isDependabot(branchName) { return branchName.toString().startsWith("dependabot/nuget") }
def isTest(branchName) { return branchName == "test" }
// def isMaster(branchName) { return branchName == "master" }

podTemplate(label: pod.label,
  containers: pod.templates + [
    containerTemplate(name: 'dotnet', image: 'ptcos/multi-netcore-sdk:0.0.2', ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
  ]
) {
    def branch = (env.BRANCH_NAME)
    def functionsProject = 'AzureFunctions'
    def publishFolder = 'publish'
    def zipName = 'publish.zip'
    def environment = isMaster(branch) ? 'Production' : 'Development'

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
            stage('Test') {
                sh """
                    dotnet test
                """
            }
        }
           if (isTest(branch) || isDependabot(branch)){
            container('powershell') {
                stage('Package') {
                    sh """
                        pwsh -command "Compress-Archive -DestinationPath $zipName -Path $functionsProject/$publishFolder/*"
                    """
                }

                if (isTest(branch) || isDependabot(branch)){
                    toAzureTestEnv {
                        def now = new Date().getTime()
                        def ciRg = 'slack-askbot-ci-' + now
                        def ciAppName = 'slack-askbot-ci-' + now
                        def mockToken = 'unused'

                        try {
                            stage('Create temporary Resource Group'){
                                sh """
                                    pwsh -command "New-AzResourceGroup -Name '$ciRg' -Location 'North Europe' -Tag @{subproject='2026956'; Description='Continuous Integration'}"
                                """
                            }
                            stage('Create test environment'){
                                sh """
                                    pwsh -command "New-AzResourceGroupDeployment -Name slack-askbot -TemplateFile Deployment/azuredeploy.json -ResourceGroupName $ciRg -appName $ciAppName -environment $environment -slackBearerToken (ConvertTo-SecureString -String $mockToken -AsPlainText -Force)"
                                """
                            }
                            stage('Publish to test environment') {
                                sh """
                                    pwsh -command "Publish-AzWebApp -ResourceGroupName $ciRg -Name $ciAppName -ArchivePath $zipName -Force"
                                """
                            }
                            stage('Create .runsettings-file acceptance tests') {
                                sh """
                                    pwsh -command "&./AcceptanceTests/Create-RunSettingsFile.ps1 -ResourceGroup $ciRg -WebAppName $ciAppName"
                                """
                            }
                            container('dotnet') {
                                stage('Acceptance tests') {
                                    sh """
                                        cd AcceptanceTests
                                        dotnet test --settings '.runsettings'
                                    """
                                }
                            }
                        }
                        finally {
                            stage('Delete test environment'){
                                sh """
                                    pwsh -command "Remove-AzResourceGroup -Name '$ciRg' -Force"
                                """
                            }
                        }
                    }
                }
            }
           }
    }
  }
