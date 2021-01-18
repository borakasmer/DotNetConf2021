package rabbitMQ

import (
	"encoding/json"
	"fmt"
	"github.com/streadway/amqp"
	"log"
	"os"
	"strconv"
	"strings"
	"webParser/parser"
	"webParser/post"
	shared2 "webParser/shared"
)

func ConsumeRabbitMQ() {
	conn, err := amqp.Dial(shared2.Config.AMQPURL)
	handleError(err, "Can't connect to AMQP")
	defer conn.Close()
	amqpChannel, err := conn.Channel()
	handleError(err, "Can't create a amqpChannel")

	defer amqpChannel.Close()

	queue, err := amqpChannel.QueueDeclare("product", false, false, false, false, nil)
	handleError(err, "Could not declare `add` queue")

	err = amqpChannel.Qos(1, 0, false)
	handleError(err, "Could not configure QoS")

	messageChannel, err := amqpChannel.Consume(
		queue.Name,
		"",
		false,
		false,
		false,
		false,
		nil,
	)
	handleError(err, "Could not register consumer")
	stopChan := make(chan bool)

	go func() {
		log.Printf("Consumer ready, PID: %d", os.Getpid())
		for d := range messageChannel {
			fmt.Println(strings.Repeat("-", 100))
			log.Printf("Received a message: %s", d.Body)

			addProduct := &shared2.AddProduct{}

			err := json.Unmarshal(d.Body, addProduct)
			if err != nil {
				log.Printf("Error decoding JSON: %s", err)
			}
			//-------------------------
			var exchange, exchangeFlt string

			//Web Parser
			//var exchange = parser.ParseWeb(shared2.Exchanges.Dolar)
			exchange = parser.ParseWeb(addProduct.ExchangeName)
			exchangeFlt = strings.Replace(exchange, ",", ".", 1)

			fmt.Println(strings.Repeat("-", 100))
			//fmt.Printf("Kur :%s - %s\n", shared2.Exchanges.Dolar,exchange)
			fmt.Printf("Kur :%s - %s\n", addProduct.ExchangeName, exchange)

			exchangeValue, err2 := strconv.ParseFloat(exchangeFlt, 64)
			if err2 != nil {
				return
			}
			//--------------------------------------------

			//Convert addProduct price to ₺ ==>TrPrice
			addProduct.TrPrice = addProduct.Price * exchangeValue
			addProduct.ExchangeValue = exchangeValue

			fmt.Printf("TR Fiyatı :%f₺\n", addProduct.TrPrice)

			log.Printf("Price %f of %s. ExchangeType : %s ConnectionID: %s", addProduct.Price, addProduct.Name, addProduct.ExchangeName,addProduct.ConnectionID)

			if err := d.Ack(false); err != nil {
				log.Printf("Error acknowledging message : %s", err)
			} else {
				log.Printf("Acknowledged message")
			}

			post.PostProduct(addProduct)
		}
	}()

	// Stop for program termination
	<-stopChan
}

func handleError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
	}

}
