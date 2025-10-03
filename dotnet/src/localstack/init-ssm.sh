#!/bin/bash
set -e

echo "🚀 Initializing LocalStack resources..."

# Create SSM Parameters
awslocal ssm put-parameter \
  --name "/DevKit/Region/eu-central-1" \
  --value "localstack-central" \
  --type String \
  --overwrite \
  --region eu-central-1

awslocal ssm put-parameter \
  --name "/DevKit/Region/eu-west-1" \
  --value "localstack-west" \
  --type String \
  --overwrite \
  --region eu-west-1
