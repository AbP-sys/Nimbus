#!/bin/bash

read -p "Enter api_id: " API_ID
read -p "Enter api_hash: " API_HASH
read -p "Enter your Telegram phone number with country code (Eg: +123456789012): " PHONE_NUMBER

# Check if a .env file already exists
if [ -f .env ]; then
  echo "WARNING: A .env file already exists. Overwriting it will overwrite\
  your encryption keys. Any data encrypted with this key will not be accessible if you lose\
  your encryption keys.\
  Do you want to proceed? (y/n)"
  read -n 1 answer
  echo
  if [ "$answer" != "y" ]; then
    echo "Aborting installation.."
    exit 0
  fi
fi

echo "API_ID=$API_ID" > .env
echo "API_HASH=$API_HASH" >> .env
echo "PHONE_NUMBER=$PHONE_NUMBER" >> .env
echo "Generating AES encryption keys..."

cd Services/Encryption/.keygen/AESKeyGen/
dotnet run
 
