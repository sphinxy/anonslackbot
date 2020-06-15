# anonslackbot

Azure-функция для slack-бота анонимной отправки сообщений с помощью слеш-команд

*TL;DR*
Если добавить в slack бота, а боту добавить webhook на конкретный канал, то можно анонимно постить сообщения через curl.
Чтобы постить сообщения через команды бота, нужен враппер, вот эта azure function.

*Боты, вебхуки и полностью анонимный курл

Идея: slack-бот может быть иметь webhook для конкретного канала, при этом в момент создания webhook видно ник того, кто его привязал, например:
"Mr.Freeman  10:18 PM added an integration to this channel: anonbot"

Это вебхук можно и нужно использовать напрямую через curl, это режим максимальной анонимности, всё напрямую через api slack, сообщения идут от бота.

https://api.slack.com/messaging/webhooks#create_a_webhook мануал.

Например:
                                         
curl --location --request POST 'https://hooks.slack.com/services/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' \
--header 'Content-Type: application/json' \
--data-raw ' {"text": "Сообщение от бота через webhook"} '

NB: curl здесь указан для канала #anonbot_test, curl для #03ch ищи в запиненных сообшениях)

*Работа с ботом через команды

Но для работы слеш-команды типа /03ch  нужен еще один шаг: команда может только посылать все данные куда-то POST-запросом, 
его нужно обработать и перепослать на вебхук. 

https://api.slack.com/interactivity/slash-commands#app_command_handling мануал

Поэтому azure-функция делает простую вещь: принимает этот POST-запрос, вытаскивает оттуда только text и посылает его на webhook. 
Тогда сообщение попадает в привязанный канал, какой именно, определяется через query-параметр webhookPostfix,
например:

curl --location --request POST 'anonslackbot.azurewebsites.net/api/AnonSlackBotCommandProcessor?webhookPostfix=T037ZKH7E/B015WFGF6V7/8vmeIRsNSScwlrb4rwNfTa7A' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'text=webhook test'

Мы специально передаём webhookPostfix, а не зашиваем его в коде, чтобы репозиторий мог быть публичным.
Ну и менять настройки проще прямо в свойствах slack-команды

*Анонимность*

Отправка сообщений напрямую через curl в webhook через api slack анонимна по природе. Ну прокси используйте, если хотите. 

Отправка сообщений через команду /03ch похуже, но сейчас azure function, используемая этой командой, открыта и деплоится напрямую из ветки master
этого репозитория, логирование body и прочего там отключено, видно только количество вызовов. Кроме того, не-админу бота кажется нет способа 
легко проверить, а на какой именно URL команда отправляет данные.
Но придётся доверять автору )

Идеи принимаются. IFTTT я пробовал уже как "прокси", например, там строго зашита приёмка value1 value2 value3, не подхожит

*Доработки*

Репозиторий открыт, тестовый канал #anonbot_test публичный, кидайте пулл-реквесты же!



