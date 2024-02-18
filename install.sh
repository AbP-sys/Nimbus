#!/bin/bash

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

echo "Generating AES encryption keys..."

cd Services/Encryption/.keygen/AESKeyGen/
dotnet run
 
