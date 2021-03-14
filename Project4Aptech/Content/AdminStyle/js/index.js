
//Lấy data về thời tiết từ API
function weatherBalloon() {
  var key = '30c8cb608c19fc16f8566525a58895f4';
  fetch('https://api.openweathermap.org/data/2.5/weather?id='+1581130+'&lang=vi'+'&appid=' + key)  
  .then(function(resp) { return resp.json() }) // Convert data to json
  .then(function(data) {
    drawWeather(data);
  })
  .catch(function() {
    // catch any errors
  }); 
}
//Gắn data vào html
function drawWeather( d ) {
  var celcius = Math.round(parseFloat(d.main.temp)-273.15);
  var celcius_min = Math.round(parseFloat(d.main.temp_min)-273.15);
  var celcius_max = Math.round(parseFloat(d.main.temp_max)-273.15);
  var fahrenheit = Math.round(((parseFloat(d.main.temp)-273.15)*1.8)+32); 
  var feel = Math.round(parseFloat(d.main.feels_like)-273.15);
	
	document.getElementById('description').innerHTML = d.weather[0].description;
  document.getElementById('temp').innerHTML = celcius + '&deg;C';
  document.getElementById('feel').innerHTML = 'Cảm giác như: '+feel+ '&deg;C';
  document.getElementById('min_max').innerHTML = celcius_min + '&deg;C - '+ celcius_max + '&deg;C';
  document.getElementById('location').innerHTML = d.name;
  document.getElementById('wind').innerHTML = 'Tốc độ gió: '+d.wind.speed+'m/s';
}
