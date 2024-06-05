import React from 'react';
import ReactDOM from 'react-dom/client';
import { configureStore } from "./store";
import App from './App';
import reportWebVitals from './reportWebVitals';

import { BrowserRouter } from "react-router-dom";
import { Provider } from "react-redux";

import { ApolloClient, InMemoryCache, ApolloProvider, HttpLink, from, ApolloLink } from '@apollo/client';
import { onError } from '@apollo/client/link/error';

const root = ReactDOM.createRoot(document.getElementById("root"));

const httpLink = new HttpLink({ uri: 'http://localhost:1761/api/graphql' });

const logoutLink = onError(({ networkError }) => {
  if (networkError.statusCode === 401) {
    console.log("401 found");
  }
});

const authMiddleware = new ApolloLink((operation, forward) => {
  const cookie = JSON.parse(sessionStorage.getItem('authUser'));
  if (cookie) {
    operation.setContext(({ headers = {} }) => ({
      headers: {
        ...headers,
        authorization: cookie.token ? `Bearer ${cookie.token}` : "",
      }
    }));
  }
  return forward(operation);
});

const client = new ApolloClient({
  cache: new InMemoryCache({ addTypename: false }),
  link: from([
    logoutLink, 
    authMiddleware,
    httpLink
  ]),
});

root.render(
  <Provider store={configureStore({})}>
    <React.Fragment>
      <BrowserRouter basename={process.env.PUBLIC_URL}>
        <ApolloProvider client={client}>
          <App />
        </ApolloProvider>
      </BrowserRouter>
    </React.Fragment>
  </Provider>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();