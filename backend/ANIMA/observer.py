import json
import time
import os

class Observer:
    def __init__(self):
        #"../../frontend.json"
        self.frontendJson = None
        self.updatedFrontendJson = None
        self.frontendJson = self.loadJson()
    
    def loadJson(self):
        with open("../../frontend.json", "r") as jsonFile:
            jsonToLoad = json.loads(jsonFile.read())
        return jsonToLoad


    def detectChanges(self):
        self.updatedFrontendJson = self.loadJson()
        changes = []
        if self.frontendJson != self.updatedFrontendJson:
            changes = self.determineChanges()
            self.frontendJson = self.updatedFrontendJson        
        return changes

    def determineChanges(self):
        old = self.frontendJson
        new = self.updatedFrontendJson

        changes = []
        
        for k,v in old.items():     
            if k in new:
                if v != new[k]:
                    changes.append((k,new[k]))
        return changes
            
def main():
    observer = Observer()
    while True:
        changes = observer.detectChanges()
        if(changes != []):
            print(changes)
        time.sleep(0.5)

if __name__ == '__main__':
    main()