URL=contoso.f1aa17387c1742aeaba9.eastus.aksapp.io
sed "s/{URL}/$URL/g" ./ingress-template.yaml > ./ingress.yaml