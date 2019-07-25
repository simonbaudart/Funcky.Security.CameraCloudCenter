# Funcky.Security.CameraCloudCenter
Camera Cloud Center is a security footage manager.
Main goal is to organize the footage uploaded by camera into a repo and copy/index/store it into another one.

## Technical specifications

- ASP Net Core 2.2
- React (with WebPack)
- Use Azure Storage to store footage

## Usage

### Workflow

1. Your cameras uploads the footage to a FTP server, it's the simplest way to get it out of your house :)
2. The Camera Cloud Center browse all footage every minute, if new footage are detected :
    1. Index the content (date, duration if video)
    2. Upload into an azure storage
    3. Delete the local file
3. The footage are now stored in a safe place. After the retention day, footage are deleted from the Azure Storage.

### Configuration

The whole solution is configured into a single JSON file.
This file is located by the connection string "ConfigFile" in appsettings.json.

Here is a sample file

```
{
    "ffprobePath": "C:\\DEV\\Tools\\ffmpeg\\bin\\ffprobe.exe",
    "users" : [
        {
            "email": "your@email.com",
            "hash" : "THEARGON2HASH"
        },
        ...
    ],
    "cameras": [
        {
            "name": "DEV",
            "key": "dev",
            "sourceDirectory": "C:\\TMP\\SBA\\Funcky.Security.CameraCloudCenter\\source\\dev",
            "storageType": "azure-storage",
            "storageConfiguration": {
                "connectionString": "AZURESTORAGECONNECTIONSTRING",
                "container": "dev",
                "retention": 30
            }
        },
        ...
    ]
}
```

- ffprobePath : path to the ffprobe.exe, used to get the data from the video files.
- users : all users that can access the web interface
    + email : the login of the user
    + hash : the Argon2 hash, see section below
- cameras : configuration for all cameras
    + name : A friendly name for the camera
    + key : A technical name, must be lower case and without any special chars, no spaces
    + sourceDirectory : Where the footage are stored, this directory is browsed every minutes, recursively
    + storageType : for now, only support "azure-storage"
    + storageConfiguration : Storage specific configuration, see below for Azure Storage
    
### Generate an Argon2 hash

To be able to login, the user must have a hash stored. When you launch the project in debug, you can post on
/api/login/generate the data below to get the hash :

```
{
  "password" : "SamplePassword"
}
```

Sample result, with hash to be stored in the configuration file :

```
{
  "hash": "$argon2id$v=19$m=65536,t=3,p=1$dTotOUNvvihAf/lpGUVKBA$XmHvh3kubHNEd2qdFpBFKIE6J2WktuaK7OD0O0NT2Go"
}
```

### Azure Storage Configuration

The actual storage is on Azure Storage, with some files created to index the footage and the actual footage.

- connectionString : the storage connection string, see Azure Portal to get it
- container : name of the container to store data for this camera
- retention : retention in days before the footage will be deleted from the storage

## Disclaimer
**USE IT AT YOUR OWN RISK**

This project is a personal project, I open it on github because I did not found anything similar but it's not supported,
you use it at your own risk !