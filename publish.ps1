dotnet publish .\TimelineLite\  -c Release  /p:GenerateRunTimeCOnfigurationFiles=True
Compress-Archive -Path C:\Work\IdeagenGCU2018\TimelineLite\TimelineLite\bin\Release\netcoreapp2.0\publish\* -DestinationPath TimelineLite.zip -Force
Compress-Archive -Path C:\Work\IdeagenGCU2018\TimelineLite\TimelineLite.Timeline\bin\Release\netcoreapp2.0\publish\* -DestinationPath TimelineLite.Timeline.zip -Force
aws s3 cp .\TimelineLite.zip s3://stewartw-test-bucket/TimelineLite.zip
aws s3 cp .\TimelineLite.Timeline.zip s3://stewartw-test-bucket/TimelineLite.Timeline.zip
