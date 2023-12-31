package ch.darki.whoishome

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.CheckBox
import android.widget.EditText
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.NavHostFragment
import ch.darki.whoishome.core.Event
import ch.darki.whoishome.dialogs.DateTimePicker
import ch.darki.whoishome.dialogs.TimePicker
import org.joda.time.DateTime


class EditEvent : Fragment() {

    private lateinit var service: ServiceManager
    private lateinit var email : String

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        super.onCreateView(inflater, container, savedInstanceState)
        val fragment = inflater.inflate(R.layout.fragment_edit_event, container, false)
        service = activity?.applicationContext as ServiceManager

        return fragment
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        val args: EditEventArgs = EditEventArgs.fromBundle(requireArguments())

        email = args.email
        service.presenceService.eventService.getEventById(args.eventId, viewLifecycleOwner.lifecycleScope) {setUpEditWindow(it, view)}
    }

    private fun setUpEditWindow(event : Event?, view : View){
        if(event == null){
            return
        }

        view.findViewById<TextView>(R.id.edit_event_name).text = event.eventName
        val editEventName = view.findViewById<EditText>(R.id.event_name_edit)
        val editStartDate = view.findViewById<EditText>(R.id.start_date_edit)
        val editEndDate = view.findViewById<EditText>(R.id.end_date_edit)
        val editRelevantForDinner = view.findViewById<CheckBox>(R.id.is_relevant_for_dinner_edit)
        val editNotAtHome = view.findViewById<CheckBox>(R.id.not_at_home_for_dinner_edit)
        val editDinnerAt = view.findViewById<EditText>(R.id.ready_for_dinner_at_edit)

        var startDate : DateTime = event.startDate
        var endDate : DateTime = event.endDate
        var dinnerAt : DateTime? = event.dinnerAt

        editEventName.setText(event.eventName)
        editStartDate.setText(event.startDate.toString("dd.MM.yyyy HH:mm"))
        editEndDate.setText(event.endDate.toString("dd.MM.yyyy HH:mm"))
        editRelevantForDinner.isChecked = event.relevantForDinner
        editNotAtHome.isChecked = event.relevantForDinner &&  event.dinnerAt == null
        editDinnerAt.setText(event.dinnerAt?.toString("HH:mm"))

        editStartDate.setOnClickListener {
            DateTimePicker(requireContext()) { d ->
                editStartDate.setText(d.toString("dd.MM.yyyy HH:mm"))
                startDate = d
            }.show()
        }

        editEndDate.setOnClickListener {
            DateTimePicker(requireContext()) { d ->
                editEndDate.setText(d.toString("dd.MM.yyyy HH:mm"))
                endDate = d
            }.show()
        }

        editDinnerAt.setOnClickListener {
            TimePicker(requireContext()){d ->
                editDinnerAt.setText(d.toString("HH:mm"))
                dinnerAt = d
            }.show()
        }

        if(event.dinnerAt != null){
            editDinnerAt.setText(event.dinnerAt.toString("HH:mm"))
        }

        view.findViewById<Button>(R.id.clear_ready_for_dinner).setOnClickListener {
            dinnerAt = null
            editDinnerAt.text.clear()
        }

        view.findViewById<Button>(R.id.cancel_edit_event).setOnClickListener {
            val action = EditEventDirections.actionEditEventViewToHome()
            NavHostFragment.findNavController(this).navigate(action)
        }

        view.findViewById<Button>(R.id.save_button).setOnClickListener {

            val relevantForDinner = editRelevantForDinner.isChecked
            val notAtHomeForDinner = editNotAtHome.isChecked

            if(!(relevantForDinner && notAtHomeForDinner)) {
                if(relevantForDinner && dinnerAt == null){
                    Toast.makeText(context, "Nicht genug Informationen", Toast.LENGTH_SHORT).show()
                    return@setOnClickListener
                }
            }
            updateEvent(editEventName.text.toString(), startDate, endDate, relevantForDinner, dinnerAt, notAtHomeForDinner, event.id)
            Toast.makeText(context, "Event geupdated", Toast.LENGTH_SHORT).show()
            val action = EditEventDirections.actionEditEventViewToHome()
            NavHostFragment.findNavController(this).navigate(action)
        }
    }

    private fun updateEvent(name: String, startDate: DateTime, endDate: DateTime,
                            relevantForDinner : Boolean, dinnerAt : DateTime?, notAtHomeForDinner : Boolean, eventId : String) {
        val person = service.logInService.currentPerson ?: return

        if(notAtHomeForDinner){
            service.presenceService.eventService.update(
                Event(
                    person,
                    name,
                    startDate,
                    endDate,
                    true,
                    null,
                    eventId
                )
            )
            return
        }

        service.presenceService.eventService.update(
            Event(
                person,
                name,
                startDate,
                endDate,
                relevantForDinner,
                dinnerAt,
                eventId
            )
        )
    }
}