echo $'<VirtualHost *:80>
    ProxyPreserveHost On
    ProxyPass / http://dotnetserver:5000
    ProxyPassReverse / http://dotnetserver:5000
</VirtualHost>' > default.conf

docker create \
	--name="apache" \
	-p 12345:80 \
	--add-host="dotnetserver:$(/sbin/ip -o -4 addr list eth0 | awk '{print $4}' | cut -d/ -f1)" \
	-v $(pwd):/config/apache/site-confs \
	linuxserver/apache && \
	docker start apache && \
	dotnet restore && \
	dotnet run -- -s http://*:5000

