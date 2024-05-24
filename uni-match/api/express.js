const express = require('express');
const sqlite3 = require('sqlite3').verbose();
const app = express();
const port = 3000;
const fs = require('fs');

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

app.listen(port, () => {
  console.log(`Server is running on http://localhost:${port}`);
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