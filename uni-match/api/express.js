const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const fs = require('fs');
const Docker = require('dockerode');
const { exec } = require('child_process');
const { debug } = require('console');
const docker = new Docker();
const app = express();
const port = 3000;

// read the json file
let users;

fs.readFile('data.json', 'utf8', (err, data) => {
  if (err) {
    console.error(err.message);
    return;
  }

  // parse the json data
  users = JSON.parse(data);
});

// connect to the database
let db = new sqlite3.Database('sqlite.db', sqlite3.OPEN_READWRITE | sqlite3.OPEN_CREATE, (err) => {
  if (err) {
    console.error(err.message);
  }
  console.log('connect to the database');

  // create the user table
  db.run(`CREATE TABLE IF NOT EXISTS users(
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    gender TEXT NOT NULL,
    location TEXT,
    university TEXT,
    interests TEXT
  )`, (err) => {
    if (err) {
      console.error(err.message);
    }
    console.log('user table created');

    // delete existing data
    db.run(`DELETE FROM users`, (err) => {
      if (err) {
        console.error(err.message);
      }
      console.log('existing data deleted');

      // insert the new data into the database
      users.forEach(user => {
        const sql = `INSERT INTO users(id, name, gender, location, university, interests) VALUES(?, ?, ?, ?, ?, ?)`;
        db.run(sql, [user.id, user.name, user.gender, user.location, user.university, user.interests], (err) => {
          if (err) {
            console.error(err.message);
          }
        });
      });

      console.log('data populated');
    });
  });
});

app.get('/', (req, res) => {
  res.send('Hello World!');
});

// handle submission
app.get('/submit', async (req, res) => {
    const repoUrl = "https://github.com/christopher192/full-stack-development-project"
    const folderName = "uni-match"
    const serviceName = "uni-match-development-project"

    try {
        const images = await docker.listImages();
        console.log(images);
        
        // create a new docker container
        const container = await docker.createContainer({
            Image: 'node',
            name: serviceName,
            Cmd: ['/bin/sh', '-c', `git clone ${repoUrl} ${folderName} /app && cd /app && npm install && npm start`], // clone github repo and start the app
            HostConfig: {
                AutoRemove: true,
                PortBindings: { '3000/tcp': [{ HostPort: `${10000 + Math.floor(Math.random() * 1000)}` }] } // map container port 3000 to a random port in the range 10000 - 11000
            }
        });

        // start the container
        await container.start();

        // get the container's ip address
        const containerInfo = await container.inspect();
        const ipAddress = containerInfo.NetworkSettings.IPAddress;

        // generate URL for accessing the service
        const serviceUrl = `http://${ipAddress}`;

        // respond with the service url
        res.json({ serviceUrl });
    } catch(error) {
        console.error('error spinning up Docker container:', error);
        res.status(500).send('error spinning up docker container');
    }
});

app.listen(port, () => {
  console.log(`server is running on http://localhost:${port}`);
});

// close the database connection when the server is stopped
process.on('SIGINT', () => {
  db.close((err) => {
    if (err) {
      return console.error(err.message);
    }
    console.log('close the database connection');
    process.exit(0);
  });
});