package parser

import (
	//"fmt"
	"github.com/PuerkitoBio/goquery"
	_ "github.com/denisenkom/go-mssqldb"
	"log"
	"net/http"
	"strings"
	"time"
)

var exchangeList map[string]string

func ParseWeb(exchange string) string {
	client := &http.Client{
		Timeout: 30 * time.Second,
	}
	res, err := client.Get("https://www.doviz.com")
	if err != nil {
		log.Fatal(err)
	}

	defer res.Body.Close()
	if res.StatusCode == 200 {
		doc, err := goquery.NewDocumentFromReader(res.Body)
		if err != nil {
			log.Fatal(err)
		} else {
			/*doc.Find("body div#extraCalls").Each(func(i int, s *goquery.Selection) {
				div := s.Find("div").First()
				id, _ := div.Attr("id")
				fmt.Printf("Div ID: %s\n", id)
			})*/
			/*doc.Find("body div").Each(func(i int, s *goquery.Selection) {
				id, exists := s.Attr("id")
				if exists && strings.TrimSpace(id)!="" {
					fmt.Printf("Div ID: %s\n", id)
				}
			})*/
		}

		data := doc.Find(".market-data .item")
		exchangeList = make(map[string]string, data.Length())
		data.Each(func(i int, s *goquery.Selection) {
			name := s.Find("a .name").Text()
			name = strings.Replace(name, " ", "", -1) //Gram Altın==>GramAltın
			kur := s.Find("a .value").Text()
			exchangeList[name] = kur
			//fmt.Printf("Kur %d: %s - %s\n", i, name, kur)
		})
		/*for key, value := range exchangeList {
			fmt.Printf("Kur :%s - %s\n", key, value)
		}*/
		//fmt.Printf("Kur :%s - %s\n", exchange, getExchangeValueByType(exchange))
		return getExchangeValueByType(exchange)
	} else {
		log.Fatalf("status code error: %d %s", res.StatusCode, res.Status)
		return "0"
	}
}

func getExchangeValueByType(key string) string {
	if val, ok := exchangeList[key]; ok {
		return val
	} else {
		return "0"
	}
}
