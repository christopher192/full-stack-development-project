package main

import (
	"fmt"
	"net/http"
	"os/exec"
)

func inferenceHandler(w http.ResponseWriter, r *http.Request) {
	text := r.URL.Query().Get("text")
	if text == "" {
		http.Error(w, "Missing 'text' query parameter", http.StatusBadRequest)
		return
	}

	cmd := exec.Command("python", "inference.py", text)
	output, err := cmd.Output()
	if err != nil {
		http.Error(w, "Inference failed: "+err.Error(), 500)
		return
	}
	fmt.Fprintf(w, "Go result: %s", output)
}

func main() {
	http.HandleFunc("/inference", inferenceHandler)
	fmt.Println("Server running at http://localhost:8080")
	http.ListenAndServe(":8080", nil)
}
