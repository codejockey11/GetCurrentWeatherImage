DEL weather.tif.bat
DEL weather.tif.gz
DEL weather.tif
DEL weather.vrt

GetCurrentWeatherImage.exe

CALL "weather.tif.bat"

ECHO OFF
CALL "C:/OSGeo4W64/bin/o4w_env.bat"
ECHO ON

REM IF EXIST "weather.tif" ( gdal_translate -of vrt -expand rgba weather.tif weather.vrt )

IF EXIST "weather.tif" ( gdal2tiles -z 4-8 -w google -r average -s EPSG:4326 -g AIzaSyCnoazHa0WEibhtQZmBqlMtXcr9LOjN5Dw weather.tif "public_html\charts\weather" )

PAUSE