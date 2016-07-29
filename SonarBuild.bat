path=%path%;C:\sonarqube-5.6.1\MSBuild.SonarQube.Runner-2.1

msbuild.sonarQube.runner begin /n:ServiceLocator /k:ServiceLocatorKey /v:%1

msbuild

msbuild.sonarQube.runner end
