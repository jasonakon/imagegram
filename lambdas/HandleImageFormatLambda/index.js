console.log("Loading function");

const aws = require("aws-sdk");
const Jimp = require("jimp");
var fs = require("fs");
var path = require("path");

const s3 = new aws.S3({ apiVersion: "2006-03-01" });

const getUrlFromBucket = (s3Bucket, fileName) => {
  return `https://${s3Bucket}.s3.ap-southeast-1.amazonaws.com/${fileName}`;
};

exports.handler = async (event, context) => {
  console.log("received event - ", event);

  const bucket = "imagegram-raw";
  const destBucket = "imagegram-final";
  let key = event.queryStringParameters.key;
  const extension = event.queryStringParameters.extension;
  let responseCode = "";
  let responseBody = {};

  const params = {
    Bucket: bucket,
    Key: key,
  };

  var keyPath = path.join("/tmp", key);
  var tempFile = fs.createWriteStream(keyPath);

  try {
    const data = (await s3.getObject(params).promise()).Body;
    console.log("File downloaded successfully");
    fs.writeFileSync(keyPath, data);
    console.log("File written successfully");
    var stats = fs.statSync(keyPath);
    var fileSizeInBytes = stats.size;
    console.log("Written file size - " + fileSizeInBytes + "KB at " + keyPath);

    let img = await Jimp.read(keyPath);
    console.log(extension);
    if (extension != "jpg") {
      key = key.substr(0, key.length - 3) + "jpg";
      keyPath = "/tmp/" + key;
      await img.writeAsync(keyPath);
    }

    var stats = fs.statSync(keyPath);
    var fileSizeInBytes = stats.size;
    console.log(
      "Resized file size at " + keyPath + " - " + fileSizeInBytes + "KB"
    );

    const keyContent = fs.readFileSync(keyPath);

    const destParams = {
      Bucket: destBucket,
      Key: key,
      Body: keyContent,
    };

    await s3.putObject(destParams).promise();
    let objectUrl = getUrlFromBucket(destBucket, key);

    console.log("Object migrated succesfully with URL : " + objectUrl);

    responseCode = 200;
    responseBody = {
      status: "pass",
      message: objectUrl,
    };
  } catch (e) {
    responseCode = 500;
    responseBody = {
      status: "fail",
      message: "Image formatted failed with error : " + e,
    };
  }

  let response = {
    statusCode: responseCode,
    body: JSON.stringify(responseBody),
  };
  console.log("response: " + JSON.stringify(response));
  return response;
};
