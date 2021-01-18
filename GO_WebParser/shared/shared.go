package shared

type Configuration struct {
	AMQPURL string
}

//If you get .Net Core service from Azure => Set Asuss Modem /Wan/Port Forwarding/
//{ Service Name:HTTP Server, Protocol :TCP, External Port:1881, Internal Port:5672(RabbitMQ),Internal IP Address:192.168.1.1}
var Config = Configuration{
	//RabbitMQ can not be reach from "Guest" user. You have to set "Test" user for Azure subscribe.
	//AMQPURL: "amqp://test:test@localhost:5672/",
	AMQPURL: "amqp://guest:guest@localhost:5672/",
}

type AddProduct struct {
	Price float64
	TrPrice float64
	Name string
	ExchangeType int
	ExchangeName string
	ExchangeValue float64
	ProductID int
	ConnectionID string
	TotalCount int
	SeriNo string
}

type exchangeType struct {
	Dolar   string
	Euro    string
	Sterlin string
	Altin   string
}

func newExchangeType() *exchangeType {
	return &exchangeType{
		Dolar:   "DOLAR",
		Euro:    "EURO",
		Sterlin: "STERLİN",
		Altin:   "GRAM ALTIN",
	}
}

var Exchanges = newExchangeType()