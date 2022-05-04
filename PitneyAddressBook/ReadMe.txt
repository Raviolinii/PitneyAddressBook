There are two Dockerfile files in this solution.
One of them is placed in api project so it would run properly.
The second one is above both projects.
It should be used from command line.
Useful commands:
	Build Tests project:	docker build -t tests --target tests -f Dockerfile .
	Build api project:	docker build -t ab -f Dockerfile .
	Run Tests 		docker run -it --rm --name tests tests
	Run api 		docker run -it --rm -p 5000:80 --name ab ab
It is required to specify port.