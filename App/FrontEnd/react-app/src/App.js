import axios from "axios";
//import axios from "axios" to make HTTP requests to the backend API
//axios are generally used in enterprise-level applications for its advanced features
import React, { useEffect, useState } from "react";
//import React is the core library for building user interfaces
//import useEffect and useState are React hooks for managing side effects and state
function App() {
  // useState hook to manage the message state
  // message will hold the data fetched from the backend API
  // setMessage is the function to update the message state
  const [message, setMessage] = useState("");

  // useEffect hook to fetch data from the backend API when the component mounts
  // fetch is used to make an HTTP GET request to the specified URL
  // fetch is browser-native and does not require an additional library
  // fetch example:
  /*useEffect(() => {
    fetch("https://localhost:7211/api/hello")
      .then((response) => response.text()) // backend returns plain string
      .then((data) => setMessage(data))
      .catch((error) => console.error("Error fetching API:", error));
  }, []);*/

  //axios is a promise-based HTTP client for the browser and Node.js
  useEffect(() =>{
    axios.get("https://localhost:7288/api/Hello")
      .then((response) => {
        setMessage(response.data); // backend returns plain string
      })
      .catch((error) => {
        console.error("Error fetching API:", error);
      });
    }, []
  );

  return (
    <div style={{ textAlign: "center", marginTop: "50px" }}>
      <h1>React Frontend</h1>
      <p>{message}</p>
    </div>
  );
}

export default App;
