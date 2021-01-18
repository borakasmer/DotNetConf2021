package main

//go get github.com/PuerkitoBio/goquery
//go get github.com/denisenkom/go-mssqldb
//go get github.com/streadway/amqp
//go get github.com/go-redis/redis/
import (
	"webParser/rabbitMQ"
)

func main() {
	//Web Parser
	//var exchange = parser.ParseWeb(shared2.Exchanges.Dolar)
	//fmt.Printf("Kur :%s - %s\n", shared2.Exchanges.Dolar,exchange)

	//RabbitMQ Consumer
	rabbitMQ.ConsumeRabbitMQ()
}
