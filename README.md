# AnonSlackBot

Azure-функция для slack-бота анонимной отправки сообщений с помощью слеш-команд

## TL;DR
Если добавить в slack бота, а боту добавить webhook на конкретный канал, то можно анонимно постить сообщения через curl.
Чтобы постить сообщения через команды бота, нужен враппер, вот эта azure function. Деплоится автоматом

[![Статус билда](https://dev.azure.com/sphinxydevops/sphinxy_nest%20github%20build/_apis/build/status/anonslackbot%20-%20CI)](https://dev.azure.com/sphinxydevops/sphinxy_nest%20github%20build/_build/latest?definitionId=1)

[![Статус деплоя](https://vsrm.dev.azure.com/sphinxydevops/_apis/public/Release/badge/ae04106c-f201-42cc-adef-6021ff99ad60/1/1)](https://dev.azure.com/sphinxydevops/sphinxy_nest%20github%20build/_release)

## Боты, вебхуки и полностью анонимный курл

Идея: slack-бот может быть иметь webhook для конкретного канала, при этом в момент создания webhook видно ник того, кто его привязал, например:
"Mr.Freeman  10:18 PM added an integration to this channel: anonbot"

Это вебхук можно и нужно использовать напрямую через curl, это режим максимальной анонимности, всё напрямую через api slack, сообщения идут от бота.

https://api.slack.com/messaging/webhooks#create_a_webhook мануал.

Например:
```                                         
curl --location --request POST 'https://hooks.slack.com/services/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' \
--header 'Content-Type: application/json' \
--data-raw ' {"text": "Сообщение от бота через webhook"} '
```

NB: curl здесь указан для уже отключенного webhook.
Актуальный curl ищи в запиненных каналах #anonbot_test и #03ch

## Работа с ботом через команды

Но для работы слеш-команды типа /03ch  нужен еще один шаг: команда может только посылать все данные куда-то POST-запросом, 
его нужно обработать и перепослать на вебхук. 

https://api.slack.com/interactivity/slash-commands#app_command_handling мануал

Поэтому azure-функция делает простую вещь: принимает этот POST-запрос, вытаскивает оттуда только text и посылает его на webhook. 
Тогда сообщение попадает в привязанный канал, какой именно, определяется через query-параметр webhookPostfix,
например:
```
curl --location --request POST 'anonslackbot.azurewebsites.net/api/AnonSlackBotCommandProcessor?webhookPostfix=T037ZKH7E/B015WFGF6V7/8vmeIRsNSScwlrb4rwNfTa7A' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'text=webhook test'
```

Мы специально передаём webhookPostfix, а не зашиваем его в коде, чтобы репозиторий мог быть публичным.
Ну и менять настройки проще прямо в свойствах slack-команды

Для поддерки тредов нужно передать префикс reply-команды, например >https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600, 
через query-параметр replyCommandPrefix, тупо, но пока так.

Sequence-диаграмма, как всё работает
http://www.plantuml.com/plantuml/uml/PP9FJi9G4CRtFSKSG5gYEn8HNB1n0GcwSrFBO1kWnRvN18q943UcnYju1IGqcbBg5URTo2a85adpaZT_-iqtlxOYfEdBeDVLcbtJwbm9z0lbSq-MmF1EB1ji744FXJ1lKJCjwGnCYS1rZK-XN7q8VAm5hbM2Hhrv2QRhkMqETJChtEEDS2GAtKLvne7jUPs69sJR6Pu-vkAQCmJwea-W5StKH4tLc5AAA0QA-PBGZ0iB5JxzETWDrdQtFJVqpafZDM7P16YnLbYfaABD8FLCAGz9L7W4D5UXccQ3AURYd7iZVebJAItLAoTJa3YKSEtIDsVUr0Jk0lH71Gvv2FSd56McONydBUzjJRVQTtv1sLBsHSamidQXYwoeMrxFOFm3nq9X_EEJMtshb5FVVB3ZOX7g5reB3odbazohRJNolXXPci5V8sCOAJyVweLXh3Z5uEP0SGQIpv853mrLq6tnd_G3

## Ответ на треды
Технически для ответа на тред в вебхук на https://hooks.slack.com/services/{webhookPostfix} нужно передать еще и ts_thread:
{"text": "тред тест", "thread_ts": "1592375586.000100"}

Чтобы ответить на тред через бота, добавьте в текст команды полную ссылку на исходное сообщение и добавьте '>' впереди, например
/03ch >https://domain.slack.com/archives/AA1B2CCC3/p5552465890058600 привет, это ответ на тред

Для тредов не забудьте в настройках передавать еще и replyCommandPrefix!

## Анонимность

Отправка сообщений напрямую через curl в webhook через api slack анонимна по природе. Ну прокси используйте, если хотите. 

Отправка сообщений через команду /03ch похуже, но сейчас azure function, используемая этой командой, открыта и деплоится напрямую из ветки master
этого репозитория, логирование body и прочего там отключено, видно только количество вызовов. Кроме того, не-админу бота кажется нет способа 
легко проверить, а на какой именно URL команда отправляет данные.
Но придётся доверять автору )

Идеи принимаются. IFTTT я пробовал уже как "прокси", например, там строго зашита приёмка value1 value2 value3, не подхожит

## Доработки

Репозиторий открыт, тестовый канал #anonbot_test публичный, кидайте пулл-реквесты же!



