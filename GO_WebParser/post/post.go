package post

import (
	"bytes"
	"encoding/json"
	"io/ioutil"
	"log"
	"net/http"
	"time"
	shared2 "webParser/shared"
)

func PostProduct(product *shared2.AddProduct) bool {
	requestBody, err := json.Marshal(product)
	if err != nil {
		log.Printf("Error encoding JSON: %s", err)
		return false
	}
	timeout := time.Duration(30 * time.Second)
	client := http.Client{Timeout: timeout}

	request, err := http.NewRequest("POST", "http://localhost:1923/Product/PushProduct", bytes.NewBuffer(requestBody))
	request.Header.Set("Content-type", "application/json")
	if err != nil {
		log.Fatalln(err)
		return false
	}
	resp, err := client.Do(request)
	if err != nil {
		log.Fatalln(err)
		return false
	}
	defer resp.Body.Close()
	body, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		log.Fatalln(err)
		return false
	}
	log.Println(string(body))
	return true
}
