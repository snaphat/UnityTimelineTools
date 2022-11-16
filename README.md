# UnityTimelineTools
Unity Plugin that adds the following features to timeline:
1. Frame Cutting and Insertion
2. Timeline and Animation View Synchronization
3. Bound Object Method Calls

## Frame Cutting and Insertion
Cut and insert frames to the overall timeline including infinite animation tracks.

![Cut_Insert](https://user-images.githubusercontent.com/5836001/201495143-d20c75a5-f624-423d-ad19-ea1ff815737f.png)

## Timeline and Animation View Synchronization
Synchronize timeline and animation views to the same timescale and location.

![Sync](https://user-images.githubusercontent.com/5836001/201495192-92c6ec90-ceea-4286-b5b7-e44ece2deec1.png)\
![Sync_Views](https://user-images.githubusercontent.com/5836001/201495269-548744c5-48a2-4cae-9329-48e8e9d57038.png)


## Bound Object Method Calls
Call methods on timeline bound objects by adding an Event Marker Receiver to the given object and adding Event Markers to its timeline track.

### Features
1. *bool*, *int*, *float*, *string*, *enum*, and *Object* parameter types
2. Method overloading
3. Multiple arguments
4. Enumerations


### Usage Images
* Adding the Event Marker Receiver script to a game object\
![Adding the Event Marker Receiver script to a game object](https://user-images.githubusercontent.com/5836001/201498890-5e60b80e-2def-4b1e-9efc-6e0cc79db133.png) ![Add_Component2](https://user-images.githubusercontent.com/5836001/201498943-347b6eb5-f334-4438-a7ae-666169b9d06e.png)

* Adding an Event Marker to a timeline\
![Adding an Event Marker to a timeline](https://user-images.githubusercontent.com/5836001/201495690-400274e9-e06a-4404-a0a9-09f79a9c24c6.png)

* Viewing the Event Marker inspector\
![Viewing the Event Marker inspector](https://user-images.githubusercontent.com/5836001/201495423-e914b968-4838-4402-98a2-3b3858f3aa12.png)

* Selecting an object method\
![Selecting an object method](https://user-images.githubusercontent.com/5836001/202037104-1f874b59-aacf-4fac-afc9-30eac99aa009.png)

* Using a multiple argument method\
![Using a multiple argument method](https://user-images.githubusercontent.com/5836001/202037734-154b3e48-f6b8-4979-8e96-84b6b15d5072.png)

* Viewing Overloaded methods\
![Viewing Overloaded methods](https://user-images.githubusercontent.com/5836001/202051465-1775f82e-982a-453a-8cac-aeb7dd6c55f9.png)

* Selecting an enumeration\
![Selecting an enumeration](https://user-images.githubusercontent.com/5836001/202051157-afdfd86b-9123-49b7-b72d-ca0751cdcaa9.png)

* Viewing an Event Marker tooltip\
![Viewing an Event Marker Tooltip](https://user-images.githubusercontent.com/5836001/201495376-3a3cb844-2910-4215-afd8-ed3f3b1e3f79.png)
