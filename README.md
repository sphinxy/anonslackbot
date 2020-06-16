# anonslackbot

Azure-������� ��� slack-���� ��������� �������� ��������� � ������� ����-������

*TL;DR*
���� �������� � slack ����, � ���� �������� webhook �� ���������� �����, �� ����� �������� ������� ��������� ����� curl.
����� ������� ��������� ����� ������� ����, ����� �������, ��� ��� azure function.

*����, ������� � ��������� ��������� ����

����: slack-��� ����� ���� ����� webhook ��� ����������� ������, ��� ���� � ������ �������� webhook ����� ��� ����, ��� ��� ��������, ��������:
"Mr.Freeman  10:18 PM added an integration to this channel: anonbot"

��� ������ ����� � ����� ������������ �������� ����� curl, ��� ����� ������������ �����������, �� �������� ����� api slack, ��������� ���� �� ����.

https://api.slack.com/messaging/webhooks#create_a_webhook ������.

��������:
                                         
curl --location --request POST 'https://hooks.slack.com/services/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' \
--header 'Content-Type: application/json' \
--data-raw ' {"text": "��������� �� ���� ����� webhook"} '

NB: curl ����� ������ ��� ������ #anonbot_test, curl ��� #03ch ��� � ���������� ����������)

*������ � ����� ����� �������

�� ��� ������ ����-������� ���� /03ch  ����� ��� ���� ���: ������� ����� ������ �������� ��� ������ ����-�� POST-��������, 
��� ����� ���������� � ����������� �� ������. 

https://api.slack.com/interactivity/slash-commands#app_command_handling ������

������� azure-������� ������ ������� ����: ��������� ���� POST-������, ����������� ������ ������ text � �������� ��� �� webhook. 
����� ��������� �������� � ����������� �����, ����� ������, ������������ ����� query-�������� webhookPostfix,
��������:

curl --location --request POST 'anonslackbot.azurewebsites.net/api/AnonSlackBotCommandProcessor?webhookPostfix=T037ZKH7E/B015WFGF6V7/8vmeIRsNSScwlrb4rwNfTa7A' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'text=webhook test'

�� ���������� ������� webhookPostfix, � �� �������� ��� � ����, ����� ����������� ��� ���� ���������.
�� � ������ ��������� ����� ����� � ��������� slack-�������

*�����������*

�������� ��������� �������� ����� curl � webhook ����� api slack �������� �� �������. �� ������ �����������, ���� ������. 

�������� ��������� ����� ������� /03ch ������, �� ������ azure function, ������������ ���� ��������, ������� � ��������� �������� �� ����� master
����� �����������, ����������� body � ������� ��� ���������, ����� ������ ���������� �������. ����� ����, ��-������ ���� ������� ��� ������� 
����� ���������, � �� ����� ������ URL ������� ���������� ������.
�� ������� �������� ������ )

���� �����������. IFTTT � �������� ��� ��� "������", ��������, ��� ������ ������ ������ value1 value2 value3, �� ��������

*���������*

����������� ������, �������� ����� #anonbot_test ���������, ������� ����-�������� ��!



