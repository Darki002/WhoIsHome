package ch.darki.whoishome.core

import android.util.Log
import com.google.firebase.firestore.FieldPath
import com.google.firebase.firestore.ktx.firestore
import com.google.firebase.ktx.Firebase
import org.joda.time.DateTime
import kotlin.coroutines.suspendCoroutine
import kotlinx.coroutines.*
import kotlinx.coroutines.tasks.await
import kotlin.coroutines.resume

class EventService {

    private val collection = "events"

    fun deleteEvent(id : String) {
        Firebase.firestore.collection(collection).document(id).delete()
    }

    fun update(event: Event) {
        val db = Firebase.firestore
        db.collection(collection).document(event.id).set(event)
            .addOnSuccessListener {
            Log.i("Update Event", "Update successfully updated")
            }
            .addOnFailureListener {
                Log.e("Update Event", "Update failed with error message $it")
            }
    }

    fun getEventById(id : String, scope: CoroutineScope, callback: (Event?) -> Unit) {
        scope.launch {
            try {
                val doc = Firebase.firestore.collection(collection)
                    .document(id).get().await()
                callback.invoke(Event.new(doc))
            }
            catch (e: Exception){
                Log.e("DB Err", e.message.toString())
            }
        }
    }

    fun createEvent(
        person: Person,
        eventName: String,
        date: DateTime,
        startTime: DateTime,
        endTime: DateTime,
        relevantForDinner : Boolean,
        dinnerAt : DateTime?,
        callback: (Boolean) -> Unit
    ) {

        val docName = person.email + " - " + DateTime.now().millis

        val event = Event(
            person,
            eventName,
            date,
            startTime,
            endTime,
            relevantForDinner,
            dinnerAt,
            docName
        )

        Firebase.firestore.collection(collection).document(docName).set(event)
            .addOnSuccessListener {callback.invoke(true)}
            .addOnFailureListener {
                Log.e("DB Err", it.message.toString())
                callback.invoke(false)
            }
    }

    fun getEventsFromPerson(person: Person, callback: (List<Event?>) -> Unit) {

        val result = ArrayList<Event?>()
        val db = Firebase.firestore

        db.collection(collection)
            .whereEqualTo(FieldPath.of("person", "email"), person.email).get()
            .addOnFailureListener {
                Log.e("DB Err", it.message.toString())
                callback.invoke(listOf())
            }
            .addOnSuccessListener {
                for (doc in it.documents){
                    result.add(Event.new(doc))
                }
                callback.invoke(result)
            }
    }

    fun getEventsForPersonByEmail(scope: CoroutineScope, email: String, callback: (EventsForPerson) -> Unit) {

        scope.launch {
            try {
                val docs = Firebase.firestore.collection(collection)
                    .whereEqualTo(FieldPath.of("person", "email"), email).get().await()

                val deferredList = docs.documents.map { doc ->
                    async {
                        suspendCoroutine { continuation ->
                            val event = Event.new(doc)
                            continuation.resume(event)
                        }
                    }
                }
                val result = getPresences(deferredList.awaitAll())
                callback.invoke(result)
            }
            catch (e: Exception){
                Log.e("DB Err", e.message.toString())
            }
        }
    }

    fun saveRepeatEvents(repeatEvent: RepeatEvent) {
        var currentDay = repeatEvent.firstDay

        while(currentDay.isBefore(repeatEvent.lastDay)){
            val date = currentDay

            createEvent(
                person = repeatEvent.person,
                eventName = repeatEvent.name,
                date = date,
                startTime = repeatEvent.startTime,
                endTime = repeatEvent.endTime,
                relevantForDinner = repeatEvent.relevantForDinner,
                dinnerAt = repeatEvent.dinnerAt
            ){
                if(!it){
                    Log.e("RepeatEvent", "Failed to create event")
                }
            }

            currentDay = currentDay.plusWeeks(1)
        }
    }

    private fun getPresences(events: List<Event?>) : EventsForPerson{

        val todaysEvents = events.stream()
            .filter { e -> e?.isToday() ?: false }
            ?.toArray<Event> { arrayOfNulls<Event>(it) }

        val thisWeeksEvents = events.stream()
            .filter { e -> e != null }
            .filter { e -> (e!!.isThisWeek() && e.date.dayOfWeek > DateTime.now().dayOfWeek) }
            ?.toArray<Event> { arrayOfNulls<Event>(it) }

        val otherEvents = events.stream()
            .filter { e -> e != null  }
            .filter { e -> e!!.date > DateTime.now() }
            .filter { e -> !e!!.isToday() && !e.isThisWeek() }
            ?.toArray<Event> { arrayOfNulls<Event>(it) }

        return EventsForPerson(
            todaysEvents?.asList() ?: ArrayList(),
            thisWeeksEvents?.asList() ?: ArrayList(),
            otherEvents?.asList() ?: ArrayList()
        )
    }

    data class EventsForPerson(
        val today: List<Event>,
        val thisWeek: List<Event>,
        val otherEvents: List<Event>
    )
}