import json
import time
import os


# Observer is used to monitor the change of json files
class Observer:
    def __init__(self, frontEndJsonPath):
        #"../../frontend.json"
        self.frontEndJsonPath = frontEndJsonPath
        #self.frontendJson = None
        self.updatedFrontendJson = None
        self.frontendJson = self.loadJson()
    
    def loadJson(self):
        with open(self.frontEndJsonPath, "r") as jsonFile:
            jsonToLoad = json.loads(jsonFile.read())
        return jsonToLoad

    '''Old detectChanges using list'''
    # def detectChanges(self):
    #     self.updatedFrontendJson = self.loadJson()
    #     changes = []
    #     if self.frontendJson != self.updatedFrontendJson:
    #         changes = self.determineChanges()
    #         self.frontendJson = self.updatedFrontendJson        
    #     return changes

    # def determineChanges(self):
    #     old = self.frontendJson
    #     new = self.updatedFrontendJson

    #     changes = []
        
    #     for k,v in old.items():     
    #         if k in new:
    #             if v != new[k]:
    #                 changes.append((k,new[k]))
    #     return changes

    '''New detectChanges using dictionary'''
    def detectChanges(self) -> dict:
        self.updatedFrontendJson = self.loadJson()
        changes = {}
        if self.frontendJson != self.updatedFrontendJson:
            changes = self.determineChanges()
            self.frontendJson = self.updatedFrontendJson
        return changes
    
    def determineChanges(self) -> dict:
        old = self.frontendJson
        new = self.updatedFrontendJson

        changes = {}

        for (k,v) in old.items():
            if (k in new) and (v != new[k]):
                changes[k] = new[k]
        return changes

            
def main():
    observer = Observer(frontEndJsonPath="../../frontend.json")
    while True:
        changes = observer.detectChanges()
        if(changes != []):
            print("changes detected: "+str(changes))
        time.sleep(0.5)

if __name__ == '__main__':
    main()