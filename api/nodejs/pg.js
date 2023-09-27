const { Pool } = require('pg');
const express = require('express');
const app = express();

const pool = new Pool({
    user: 'postgres',
    host: 'localhost',
    database: 'DUMMY-DB',
    password: 'Password123',
    port: 5432,
});

pool.query('SELECT NOW()', (err, result) => {
    if (err) {
        console.error('Error connecting to PostgreSQL:', err);
    } else {
        console.log('Connected to PostgreSQL:', result.rows[0].now);
    }
});

app.get('/queryDatabase', (req, res) => {
    pool.query('SELECT NOW()', (err, result) => {
        if (err) {
            console.error('Error querying PostgreSQL:', err);
            res.status(500).json({ error: 'Database error' });
        } else {
            res.json({ currentTime: result.rows[0].now });
        }
    });
});

const port = 3000;

app.listen(port, () => {
    console.log(`Server is running on port ${port}`);
});