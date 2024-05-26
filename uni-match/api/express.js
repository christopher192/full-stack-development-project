const fs = require('fs');
const redis = require('redis');
const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const Docker = require('dockerode');
const OAuthServer = require('oauth2-server');
const bodyParser = require('body-parser');
const { exec } = require('child_process');
const { debug } = require('console');
const cors = require('cors');

const redisClient = redis.createClient({
  host: '127.0.0.1',
  port: '6379',
});

redisClient.on('error', err => console.log('Redis Client Error', err));

redisClient.connect();

const { Request, Response } = OAuthServer;
const docker = new Docker();
const app = express();
const port = 3000;

// enable cors for all routes
app.use(cors());

// use body-parser middleware
app.use(bodyParser.urlencoded({ extended: false }));

// create a dummy oauth2 server instance
const oauth = new OAuthServer({
  model: {
    getAccessToken: (accessToken) => {
      // try {
      //   const token = await redisClient.get(accessToken);
      //   return token ? JSON.parse(token) : null;
      // } catch (error) {
      //   console.error('Error retrieving access token from Redis:', error);
      //   return null;
      // }
    },
    getClient: (clientId, clientSecret) => {
      console.log('getClient called with:', clientId, clientSecret);
      if (clientId === 'dummy_client' && clientSecret === 'dummy_secret') {
        return {
          id: 'dummy_client',
          redirectUris: [],
          grants: ['password', 'refresh_token', 'client_credentials']
        };
      }
        return null;
    },
    getUser: (username, password) => {
      if (username === 'test' && password === 'test') {
        return { id: 'test_user' };
      }
      return null;
    },
    saveToken: (token, client, user) => {
      // redisClient.set(token.accessToken, JSON.stringify({ token, client, user }), (error) => {
      //   if (error) {
      //     console.error('Error saving access token to Redis:', error);
      //   } else {
      //     console.log('Access token saved to Redis:', token.accessToken);
      //   }
      // });
      return {
        accessToken: token.accessToken,
        accessTokenExpiresAt: token.accessTokenExpiresAt,
        refreshToken: token.refreshToken,
        refreshTokenExpiresAt: token.refreshTokenExpiresAt,
        client: client,
        user: user,
      };
    },
    getRefreshToken: (refreshToken) => {
      console.log('getRefreshToken called with:', refreshToken);
      if (refreshToken === 'dummy_refresh_token') {
        return {
          refreshToken: 'dummy_refresh_token',
          refreshTokenExpiresAt: new Date(Date.now() + 3600000),
          client: { id: 'dummy_client' },
          user: {},
          scope: 'read write'
        };
      }
      return null;
    },
    revokeToken: (token) => {
      console.log('revokeToken called with:', token);
      return token.refreshToken === 'dummy_refresh_token';
    }
  },
});

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
    } catch(error) {
      console.error('error spinning up docker container:', error);
      res.status(500).send('error spinning up docker container');
    }
});

app.post('/oauth/token', (req, res) => {
  const request = new Request(req);
  const response = new Response(res);

  oauth
    .token(request, response)
    .then((token) => {
      return res.json(token);
    })
    .catch((err) => {
      return res.status(500).json(err);
    });
});

// protected endpoint
app.get('/protected', (req, res, next) => {
  const request = new Request(req);
  const response = new Response(res);

  oauth.authenticate(request, response)
    .then((token) => {
      res.json({ message: 'you have accessed a protected endpoint!', token });
    })
    .catch((err) => {
      res.status(err.code || 500).json(err);
    });
});

app.get('/users', (req, res) => {
  const sql = `SELECT * FROM users`;
  db.all(sql, [], (err, rows) => {
    if (err) {
      console.error(err.message);
      return res.status(500).send('Internal Server Error');
    }
    res.json(rows);
  });
});

app.listen(port, () => {
  console.log(`server is running on http://localhost:${port}`);
});

// close the database connection and redis when the server is stopped
process.on('SIGINT', () => {
  db.close((err) => {
    if (err) {
      return console.error(err.message);
    }
    console.log('close the database connection');
    process.exit(0);
  });
  redisClient.quit();
});