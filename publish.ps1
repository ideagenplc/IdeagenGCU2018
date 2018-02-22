dotnet publish .\TimelineLite\  -c Release  -f netcoreapp2.0 /p:GenerateRunTimeCOnfigurationFiles=True

Compress-Archive -Path C:\Work\IdeagenGCU2018\TimelineLite\TimelineLite\bin\Release\netcoreapp2.0\publish\* -DestinationPath TimelineLite.zip -Force
Compress-Archive -Path C:\Work\IdeagenGCU2018\TimelineLite\TimelineLite.Timeline\bin\Release\netcoreapp2.0\publish\* -DestinationPath TimelineLite.Timeline.zip -Force
Compress-Archive -Path C:\Work\IdeagenGCU2018\TimelineLite\TimelineLite.TimelineEvent\bin\Release\netcoreapp2.0\publish\* -DestinationPath TimelineLite.TimelineEvent.zip -Force
Compress-Archive -Path C:\Work\IdeagenGCU2018\TimelineLite\TimelineLite.TimelineEventAttachment\bin\Release\netcoreapp2.0\publish\* -DestinationPath TimelineLite.TimelineEventAttachment.zip -Force
Compress-Archive -Path C:\Work\IdeagenGCU2018\TimelineLite\TimelineLite.General\bin\Release\netcoreapp2.0\publish\* -DestinationPath TimelineLite.General.zip -Force

aws s3 cp .\TimelineLite.zip s3://stewartw-test-bucket/TimelineLite.zip
aws s3 cp .\TimelineLite.Timeline.zip s3://stewartw-test-bucket/TimelineLite.Timeline.zip
aws s3 cp .\TimelineLite.TimelineEvent.zip s3://stewartw-test-bucket/TimelineLite.TimelineEvent.zip
aws s3 cp .\TimelineLite.TimelineEventAttachment.zip s3://stewartw-test-bucket/TimelineLite.TimelineEventAttachment.zip
aws s3 cp .\TimelineLite.General.zip s3://stewartw-test-bucket/TimelineLite.General.zip
