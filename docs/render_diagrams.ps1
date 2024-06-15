# render the diagrams in the diagrams folder using PlantUML

Write-Output "Rendering PlantUML diagrams to ../images/*.png files using a docker container"

docker run --rm -v ${PWD}:/data dstockhammer/plantuml -tpng -o ./images .


